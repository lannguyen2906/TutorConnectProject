import { storage } from "@/utils/services/firebase";
import { UploadFile } from "antd";
import {
  deleteObject,
  getDownloadURL,
  listAll,
  ref,
  uploadBytesResumable,
} from "firebase/storage";
import { useState } from "react";

export interface UseUploadFileResult {
  selectedFiles: UploadFile[];
  selectedVideo: UploadFile | null;
  handleFileChange: (e: UploadFile[]) => void;
  handleVideoChange: (video: UploadFile) => void;
  removeFile: (uid: string) => void;
  uploadFiles: (folderName: string) => Promise<string[]>;
  uploadVideo: (folderName: string) => Promise<string | null>;
}

const useUploadFileEasy = (): UseUploadFileResult => {
  const [selectedFiles, setSelectedFiles] = useState<UploadFile[]>([]);
  const [selectedVideo, setSelectedVideo] = useState<UploadFile | null>(null);

  // Handle files change
  const handleFileChange = (fileList: UploadFile[]) => {
    if (fileList.length > 0) {
      setSelectedFiles(fileList);
    }
  };

  // Handle video change
  const handleVideoChange = (video: UploadFile) => {
    if (video) {
      setSelectedVideo(video);
    }
  };

  // Remove file from the list
  const removeFile = (uid: string) => {
    setSelectedFiles((prev) => prev.filter((f) => f.uid != uid));
  };

  // Upload files to Firebase
  const uploadFiles = async (folderName: string): Promise<string[]> => {
    const fileUrls: string[] = [];

    const folderRef = ref(storage, `${folderName}/files`);

    if (folderName.startsWith("contracts")) {
      // Xóa tất cả các tệp trong thư mục contracts/files
      const listResult = await listAll(folderRef);
      const deletePromises = listResult.items.map((item) => deleteObject(item));
      await Promise.all(deletePromises); // Đợi tất cả các tệp được xóa
    }

    for (const file of selectedFiles) {
      if (!file.originFileObj) {
        fileUrls.push(file.url!);
        continue;
      }
      const timestamp = Date.now();
      const fileRef = ref(folderRef, `${timestamp}_${file.uid}`);

      const uploadTask = uploadBytesResumable(fileRef, file.originFileObj!);

      await new Promise((resolve, reject) => {
        uploadTask.on("state_changed", null, reject, async () => {
          const downloadURL = await getDownloadURL(fileRef);
          fileUrls.push(downloadURL);
          resolve(null);
        });
      });
    }

    return fileUrls;
  };
  // Upload video to Firebase
  const uploadVideo = async (folderName: string): Promise<string | null> => {
    if (!selectedVideo || !selectedVideo.originFileObj) {
      return null;
    }

    const timestamp = Date.now();
    const videoRef = ref(
      storage,
      `${folderName}/video/${timestamp}_${selectedVideo.uid}`
    );

    const uploadTask = uploadBytesResumable(
      videoRef,
      selectedVideo.originFileObj!
    );

    return new Promise((resolve, reject) => {
      uploadTask.on("state_changed", null, reject, async () => {
        const downloadURL = await getDownloadURL(videoRef);
        resolve(downloadURL);
      });
    });
  };

  return {
    selectedFiles,
    selectedVideo,
    handleFileChange,
    handleVideoChange,
    removeFile,
    uploadFiles,
    uploadVideo,
  };
};

export default useUploadFileEasy;
