import { useState } from "react";
import {
  ref,
  uploadBytesResumable,
  getDownloadURL,
  deleteObject,
  StorageReference,
} from "firebase/storage";
import { storage } from "@/utils/services/firebase";

interface UseUploadFileResult {
  tempFileURL: string | null;
  handleFileChange: (e: React.ChangeEvent<HTMLInputElement>) => Promise<void>;
  handleCancelOrChangeImage: () => Promise<void>;
  moveFile: (folderName: string) => Promise<string | null>;
  progress: number;
}

const useUploadFile = ({
  tempFileURLDefault,
}: {
  tempFileURLDefault: string | null;
}): UseUploadFileResult => {
  const [tempFileURL, setTempFileURL] = useState<string | null>(
    tempFileURLDefault
  );
  const [tempFileRef, setTempFileRef] = useState<StorageReference | null>(null);
  const [progress, setProgress] = useState<number>(0);

  // Hàm upload file tạm thời lên Firebase
  const uploadTempFile = async (file: File) => {
    const timestamp = Date.now();

    const tempRef = ref(storage, `temp-files/${timestamp}_${file.name}`);
    setTempFileRef(tempRef);

    const uploadTask = uploadBytesResumable(tempRef, file);

    uploadTask.on(
      "state_changed",
      (snapshot) => {
        const progress =
          (snapshot.bytesTransferred / snapshot.totalBytes) * 100;
        setProgress(progress);
      },
      (error) => {
        console.error("Lỗi khi upload file:", error);
      },
      async () => {
        const downloadURL = await getDownloadURL(uploadTask.snapshot.ref);
        setTempFileURL(downloadURL);
      }
    );
  };

  // Hàm xử lý khi người dùng chọn file
  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = e.target.files ? e.target.files[0] : null;
    if (selectedFile) {
      // Nếu đã có file tạm thời trước đó, xóa file cũ đi
      if (tempFileRef) {
        await deleteTempFile(tempFileRef);
      }

      // Upload file mới lên Firebase
      await uploadTempFile(selectedFile);
    }
  };

  // Hàm xóa file tạm thời khỏi Firebase
  const deleteTempFile = async (fileRef: StorageReference) => {
    try {
      await deleteObject(fileRef);
      console.log("File tạm thời đã được xóa.");
      setTempFileURL(null);
      setTempFileRef(null);
    } catch (error) {
      console.error("Lỗi khi xóa file tạm thời:", error);
    }
  };

  // Hàm chuyển file từ `temp-files` sang `posts-files`
  const moveFile = async (folderName: string): Promise<string | null> => {
    if (tempFileRef) {
      try {
        const newRef = ref(storage, `${folderName}/${tempFileRef.name}`);
        const tempURL = await getDownloadURL(tempFileRef);
        const response = await fetch(tempURL);
        const fileBlob = await response.blob();

        // Upload file mới
        await uploadBytesResumable(newRef, fileBlob);

        // Xóa file tạm sau khi upload thành công
        await deleteTempFile(tempFileRef);

        console.log(
          `File đã được chuyển sang ${folderName}: ${tempFileRef.name}`
        );

        // Trả về URL của file sau khi upload
        const downloadURL = await getDownloadURL(newRef);
        return downloadURL;
      } catch (error) {
        console.error("Lỗi khi chuyển file: ", error);
        return null; // Xử lý lỗi nếu cần
      }
    }

    return null;
  };

  // Hàm hủy upload hoặc đổi ảnh
  const handleCancelOrChangeImage = async () => {
    if (tempFileRef) {
      await deleteTempFile(tempFileRef);
    }
  };

  return {
    tempFileURL,
    handleFileChange,
    handleCancelOrChangeImage,
    moveFile,
    progress,
  };
};

export default useUploadFile;
