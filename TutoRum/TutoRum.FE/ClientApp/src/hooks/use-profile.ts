import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import { UpdateUserDTO } from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import { useMutation, useQueryClient } from "react-query";

export const useUpdateProfile = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: UpdateUserDTO) =>
      tConnectService.api.userUpdateProfileCreate(data),
    {
      onSuccess: async (_, args) => {
        console.log("Mutation success! Invalidating queries...");

        await queryClient.invalidateQueries(cacheKeysUtil.currentProfile()),
          console.log("Invalidated query: currentProfile");
      },
    }
  );
};
