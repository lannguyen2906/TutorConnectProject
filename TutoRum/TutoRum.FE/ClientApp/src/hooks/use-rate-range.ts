import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import { RateRange } from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "react-query";

export const useAllRateRanges = (): UseQueryResult<RateRange[]> =>
  useQuery(cacheKeysUtil.getAllRateRanges(), async (): Promise<any> => {
    const response = await tConnectService.api.rateRangeGetAllRateRangesList();
    const { data } = response;
    return data.data;
  });

export const useRateRangeById = (id: number): UseQueryResult<RateRange> =>
  useQuery(cacheKeysUtil.getRateRangeById(id), async (): Promise<any> => {
    const response = await tConnectService.api.rateRangeGetRateRangeByIdList({
      id,
    });
    const { data } = response;
    return data.data;
  });

export const useCreateRateRange = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: RateRange) =>
      tConnectService.api.rateRangeCreateRateRangeCreate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.getAllRateRanges()),
        ]);
      },
    }
  );
};

export const useUpdateRateRange = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: RateRange) =>
      tConnectService.api.rateRangeUpdateRateRangeUpdate(data, {
        id: data.id,
      }),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.getAllRateRanges()),
        ]);
      },
    }
  );
};

export const useDeleteRateRange = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (id: number) =>
      tConnectService.api.rateRangeDeleteRateRangeDelete({
        id,
      }),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.getAllRateRanges()),
        ]);
      },
    }
  );
};
