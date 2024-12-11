import { toast } from "@/hooks/use-toast";
import { AxiosInstance } from "axios";

export type AttachInterceptorFunction = (axiosInstance: AxiosInstance) => void;

const skipToastForEndpoints = ["/api/Accounts/current"];

export const attachCommonErrorInterceptor: AttachInterceptorFunction = (
  axiosInstance
) => {
  axiosInstance.interceptors.response.use(
    (response) => response,
    (error) => {
      if (skipToastForEndpoints.includes(error.config.url!))
        return Promise.reject(error);
      toast({
        title: "Error",
        description: error.response?.data.message,
        variant: "destructive",
      });
    }
  );
};

export const attachStatusCodeErrorInterceptor: AttachInterceptorFunction = (
  axiosInstance
) => {
  axiosInstance.interceptors.response.use((response) => {
    if (response?.data?.statusCode && response.data.statusCode !== 200) {
      toast({
        title: "Error",
        description: response?.data,
        variant: "destructive",
      }); // This will stop the next interceptor (attachCommonErrorInterceptor) from running
      // And throw the error to the catch block of the caller
      throw new Error(response?.data?.message);
    }
    return response;
  });
};
