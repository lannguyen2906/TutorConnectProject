import { storage } from "@/utils/services/firebase";
import { ref, listAll, getDownloadURL } from "firebase/storage";
import { useState, useEffect } from "react";

export interface UseListFilesResult {
  fileUrls: string[];
  loading: boolean;
  error: string | null;
}

export const getFilesByUrl = async (folderPath: string): Promise<string[]> => {
  try {
    const folderRef = ref(storage, folderPath);
    const result = await listAll(folderRef);
    const urls = await Promise.all(
      result.items.map((itemRef) => getDownloadURL(itemRef))
    );
    return urls;
  } catch (err) {
    console.log(err);
    return [];
  }
};

const useListFiles = (folderPath: string): UseListFilesResult => {
  const [fileUrls, setFileUrls] = useState<string[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchFiles = async () => {
      setLoading(true);
      setError(null);
      try {
        const folderRef = ref(storage, folderPath);
        const result = await listAll(folderRef);
        const urls = await Promise.all(
          result.items.map((itemRef) => getDownloadURL(itemRef))
        );
        setFileUrls(urls);
      } catch (err) {
        setError("Failed to load files.");
      } finally {
        setLoading(false);
      }
    };

    if (folderPath) {
      fetchFiles();
    }
  }, [folderPath]);

  return {
    fileUrls,
    loading,
    error,
  };
};

export default useListFiles;
