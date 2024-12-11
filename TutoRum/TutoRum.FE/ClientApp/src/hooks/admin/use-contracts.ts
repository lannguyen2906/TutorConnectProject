import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import { ContractDtoPagedResult } from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "react-query";

export const useAdminContractList = (
  page: number,
  size: number
): UseQueryResult<ContractDtoPagedResult> =>
  useQuery(
    cacheKeysUtil.adminContractList(page, size),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.tutorLearnerSubjectApiContractsList({
          pageIndex: page - 1,
          pageSize: size,
        });
      const { data } = response;
      return data.data;
    }
  );

export const useVerifyContract = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async ({
      contractId,
      isVerified,
      reason,
    }: {
      contractId: number;
      isVerified: boolean;
      reason: string;
    }) =>
      tConnectService.api.tutorLearnerSubjectApiTutorlearnerVerifycontractCreate(
        { tutorLearnerSubjectId: contractId, isVerified, reason }
      ),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.adminContractList(1, 5)),
          queryClient.invalidateQueries(cacheKeysUtil.adminMenuAction()),
        ]);
      },
    }
  );
};
