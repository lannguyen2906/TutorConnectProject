import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import {
  AdddBillingEntryDTO,
  BillDetailsDTO,
  BillDetailsDTOPagedResult,
  BillDTOS,
  BillingEntryDTOS,
} from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "react-query";

export const useBillingEntriesByTutorLearnerSubject = (
  pageNumber: number,
  pageSize: number,
  tutorLearnerSubjectId: number
): UseQueryResult<BillingEntryDTOS> =>
  useQuery(
    cacheKeysUtil.billingEntriesByTutorLearnerSubject(
      Number(tutorLearnerSubjectId)
    ),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.billingEntryGetAllBillingEntriesByTutorLearnerSubjectIdList(
          {
            pageIndex: pageNumber - 1,
            pageSize: pageSize,
            tutorLearnerSubjectId: tutorLearnerSubjectId,
          }
        );
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
      refetchInterval: 3600000,
    }
  );

export const useAddBillingEntry = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: AdddBillingEntryDTO) =>
      tConnectService.api.billingEntryAddBillingEntryCreate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.billingEntriesByTutorLearnerSubject(
              Number(args.tutorLearnerSubjectId!)
            )
          ),
        ]);
      },
    }
  );
};

export const useUpdateBillingEntry = (billingEntryId: number) => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: AdddBillingEntryDTO) =>
      tConnectService.api.billingEntryUpdateBillingEntryUpdate(data, {
        billingEntryId: billingEntryId,
      }),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.billingEntriesByTutorLearnerSubject(
              args.tutorLearnerSubjectId!
            )
          ),
        ]);
      },
    }
  );
};

export const useGenBillFromBillingEntries = (tutorLearnerSubjectId: number) => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: number[]) =>
      tConnectService.api.billGenerateBillFromBillingEntriesCreate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.billingEntriesByTutorLearnerSubject(
              tutorLearnerSubjectId
            )
          ),
        ]);
      },
    }
  );
};

export const useBillsByTutorLearnerSubject = (
  pageNumber: number,
  pageSize: number,
  tutorLearnerSubjectId: number
): UseQueryResult<BillDetailsDTOPagedResult> =>
  useQuery(
    cacheKeysUtil.billsByTutorLearnerSubject(tutorLearnerSubjectId),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.billGetBillByTutorLearnerSubjectIdList({
          pageIndex: pageNumber - 1,
          pageSize: pageSize,
          tutorLearnerSubjectId: tutorLearnerSubjectId,
        });
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
    }
  );

export const useBillsByTutor = (
  pageNumber: number,
  pageSize: number
): UseQueryResult<BillDetailsDTOPagedResult> =>
  useQuery(
    cacheKeysUtil.billsByTutor(),
    async (): Promise<any> => {
      const response = await tConnectService.api.billGetBillByTutorList({
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

export const useBillHtmlById = (
  billId: number
): UseQueryResult<BillingEntryDTOS> =>
  useQuery(
    cacheKeysUtil.billHtmlById(billId),
    async (): Promise<any> => {
      const response = await tConnectService.api.billApiBillViewBillHtmlList({
        billId,
      });
      const { data } = response;
      return data;
    },
    {
      keepPreviousData: true,
    }
  );

export const useBillDetailById = (
  billId: number
): UseQueryResult<BillDetailsDTO> =>
  useQuery(
    cacheKeysUtil.billDetailById(billId),
    async (): Promise<any> => {
      const response = await tConnectService.api.billGetBillDetailByIdList({
        billId,
      });
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
    }
  );

export const useApproveBill = (billId: number) => {
  const queryClient = useQueryClient();

  return useMutation(
    async () =>
      tConnectService.api.billApproveBillCreate({
        billId,
      }),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.billDetailById(billId)),
        ]);
      },
    }
  );
};

export const useDeleteBill = (
  billId: number,
  tutorLearnerSubjectId: number
) => {
  const queryClient = useQueryClient();

  return useMutation(
    async () =>
      tConnectService.api.billDeleteBillDelete({
        billId,
      }),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.billsByTutorLearnerSubject(tutorLearnerSubjectId)
          ),
        ]);
      },
    }
  );
};

export const useDeleteBillingEntry = (tutorLearnerSubjectId: number) => {
  const queryClient = useQueryClient();

  return useMutation(
    async (ids: number[]) =>
      tConnectService.api.billingEntryDeleteBillingEntryDelete(ids),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.billingEntriesByTutorLearnerSubject(
              Number(tutorLearnerSubjectId)
            )
          ),
        ]);
      },
    }
  );
};
