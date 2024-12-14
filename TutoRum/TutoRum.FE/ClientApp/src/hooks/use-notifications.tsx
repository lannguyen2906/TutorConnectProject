import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import { NotificationDtos, UserTokenDto } from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "react-query";

export const useNotifications = (
  pageIndex: number,
  pageSize: number
): UseQueryResult<NotificationDtos> =>
  useQuery(cacheKeysUtil.notifications(), async (): Promise<any> => {
    const response =
      await tConnectService.api.notificationGetAllNotificationsList({
        pageIndex: pageIndex,
        pageSize: pageSize,
      });
    const { data } = response;
    return data.data;
  });

export const useMarkAsRead = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (ids: number[]) =>
      tConnectService.api.notificationMarkNotificationsAsReadUpdate(ids),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.notifications()),
        ]);
      },
    }
  );
};

export const useSaveFCMToken = () => {
  return useMutation(async (userTokenDto: UserTokenDto) =>
    tConnectService.api.notificationSaveFcmTokenCreate(userTokenDto)
  );
};
