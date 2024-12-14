import { useAppContext } from "@/components/provider/app-provider";
import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import { AdminHomePageDTO, VerificationStatusDto } from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "react-query";

export const useAdminTutorList = (
  pageNumber: number,
  pageSize: number,
  search?: string,
  status?: string,
  startDate?: string,
  endDate?: string
): UseQueryResult<AdminHomePageDTO> =>
  useQuery(
    cacheKeysUtil.adminTutorList(
      pageNumber - 1,
      pageSize,
      search,
      status,
      startDate,
      endDate
    ),
    async (): Promise<any> => {
      const response = await tConnectService.api.adminGetAdminListByTutorList({
        Search: search,
        Status: status,
        StartDate: startDate,
        EndDate: endDate,
        index: pageNumber - 1,
        size: pageSize,
      });
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
    }
  );

export const useVerifyStatus = () => {
  const queryClient = useQueryClient();
  const { user } = useAppContext();

  return useMutation(
    async (data: VerificationStatusDto) =>
      tConnectService.api.adminVerificationStatusCreate(data),
    {
      onSuccess: async (_, args) => {
        switch (args.entityType) {
          case 0:
          case 1:
          case 2:
            await Promise.all([
              queryClient.invalidateQueries(
                cacheKeysUtil.getAllTutorRequestsAdmin(0, 5)
              ),
            ]);
          case 4:
            await Promise.all([
              queryClient.invalidateQueries(cacheKeysUtil.tutorList()),
              queryClient.invalidateQueries(
                cacheKeysUtil.tutorDetail(args.guidId ?? "")
              ),
            ]);
            break;
          case 3:
            await Promise.all([
              queryClient.invalidateQueries(
                cacheKeysUtil.subjectDetailList(user?.id!, "viewLearners")
              ),
            ]);
        }
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.tutorList()),
          queryClient.invalidateQueries(
            cacheKeysUtil.tutorDetail(args.guidId ?? "")
          ),
          queryClient.invalidateQueries(cacheKeysUtil.adminMenuAction()),
        ]);
      },
    }
  );
};
