import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import {
  DashboardChartData,
  DashboardKeyMetricsDto,
} from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import { useQuery, UseQueryResult } from "react-query";

export const useDashboardChartData = (): UseQueryResult<DashboardChartData> =>
  useQuery(
    cacheKeysUtil.dashboardCharts(),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.adminGetDashboardChartsDataList();
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
      refetchInterval: 3600000,
    }
  );

export const useDashboardKeyMetrics =
  (): UseQueryResult<DashboardKeyMetricsDto> =>
    useQuery(
      cacheKeysUtil.dashboardKeyMetrics(),
      async (): Promise<any> => {
        const response =
          await tConnectService.api.adminGetDashboardKeyMetricsList();
        const { data } = response;
        return data.data;
      },
      {
        keepPreviousData: true,
        refetchInterval: 3600000,
      }
    );
