import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import { AdminMenuAction } from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import { useQuery, UseQueryResult } from "react-query";

export const useAdminMenuAction = (): UseQueryResult<AdminMenuAction[]> =>
  useQuery(cacheKeysUtil.adminMenuAction(), async (): Promise<any> => {
    const response = await tConnectService.api.adminGetAdminMenuActionList();
    const { data } = response;
    return data.data;
  });
