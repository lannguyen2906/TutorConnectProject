import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import {
  CreatePaymentRequestDTO,
  PaymentRequestDTOPagedResult,
  WalletOverviewDto,
} from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "react-query";

export const useAllPaymentRequests = (
  pageNumber: number,
  pageSize: number
): UseQueryResult<PaymentRequestDTOPagedResult> =>
  useQuery(
    cacheKeysUtil.getAllPaymentRequests(pageNumber - 1, pageSize),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.paymentRequestGetListPaymentRequestsList({
          pageIndex: pageNumber - 1,
          pageSize: pageSize,
        });
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
    }
  );

export const usePaymentRequestsByTutor = (
  pageNumber: number,
  pageSize: number
): UseQueryResult<PaymentRequestDTOPagedResult> =>
  useQuery(
    cacheKeysUtil.getPaymentRequestsByTutor(pageNumber - 1, pageSize),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.paymentRequestGetListPaymentRequestsByTutorList(
          {
            pageIndex: pageNumber - 1,
            pageSize: pageSize,
          }
        );
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
    }
  );

export const useCreatePaymentRequest = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (createPaymentRequestDTO: CreatePaymentRequestDTO) =>
      tConnectService.api.paymentRequestCreatePaymentRequestCreate(
        createPaymentRequestDTO
      ),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.getPaymentRequestsByTutor(0, 10)
          ),
          queryClient.invalidateQueries(cacheKeysUtil.getWalletOverview()),
          queryClient.invalidateQueries(cacheKeysUtil.currentProfile()),
        ]);
      },
    }
  );
};

export const useDeletePaymentRequest = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (paymentRequestId: number) =>
      tConnectService.api.paymentRequestDeletePaymentRequestByIdDelete(
        paymentRequestId
      ),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.getPaymentRequestsByTutor(0, 10)
          ),
          queryClient.invalidateQueries(cacheKeysUtil.getWalletOverview()),
          queryClient.invalidateQueries(cacheKeysUtil.currentProfile()),
        ]);
      },
    }
  );
};

export const useApprovePaymentRequest = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (paymentRequestId: number) =>
      tConnectService.api.paymentRequestApprovePaymentRequestUpdate(
        paymentRequestId
      ),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.getAllPaymentRequests(0, 10)
          ),
        ]);
      },
    }
  );
};

export const useRejectPaymentRequest = (paymentRequestId: number) => {
  const queryClient = useQueryClient();

  return useMutation(
    async (rejectionReason: string) =>
      tConnectService.api.paymentRequestRejectPaymentRequestUpdate(
        paymentRequestId,
        { rejectionReason }
      ),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.getAllPaymentRequests(0, 10)
          ),
        ]);
      },
    }
  );
};

export const useWalletOverview = (): UseQueryResult<WalletOverviewDto> =>
  useQuery(
    cacheKeysUtil.getWalletOverview(),
    async (): Promise<any> => {
      const response = await tConnectService.api.tutorMyWalletOverviewList();
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
    }
  );
