import { useAppContext } from "@/components/provider/app-provider";
import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import {
  ListTutorRequestDTOPagedResult,
  ListTutorRequestForTutorDtoPagedResult,
  TutorRequestDTO,
  TutorRequestDTOPagedResult,
  TutorRequestWithTutorsDTO,
} from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "react-query";

export const useAllTutorRequests = (
  pageNumber: number,
  pageSize: number,
  search?: string,
  cityId?: string,
  districtId?: string,
  minFee?: number,
  maxFee?: number,
  rateRangeId?: number,
  subject?: string,
  tutorQualificationId?: number,
  tutorGender?: string
): UseQueryResult<TutorRequestDTOPagedResult> =>
  useQuery(
    cacheKeysUtil.getAllTutorRequests(
      pageNumber - 1,
      pageSize,
      search,
      cityId,
      districtId,
      minFee,
      maxFee,
      rateRangeId,
      subject,
      tutorQualificationId,
      tutorGender
    ),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.tutorRequestGetAllTutorRequestsList({
          CityId: cityId,
          DistrictId: districtId,
          MaxFee: maxFee,
          MinFee: minFee,
          RateRangeId: rateRangeId,
          Search: search,
          Subject: subject,
          TutorGender: tutorGender,
          TutorQualificationId: tutorQualificationId,
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

export const useTutorRequestsAdmin = (
  pageNumber: number,
  pageSize: number
): UseQueryResult<ListTutorRequestDTOPagedResult> =>
  useQuery(
    cacheKeysUtil.getAllTutorRequestsAdmin(pageNumber - 1, pageSize),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.tutorRequestGetTutorRequestsAdminList({
          pageIndex: pageNumber - 1,
          pageSize: pageSize,
        });
      const { data } = response;
      return data;
    },
    {
      keepPreviousData: true,
    }
  );

export const useAllTutorRequestsByLearner = (
  learnerId: string,
  pageNumber: number,
  pageSize: number
): UseQueryResult<ListTutorRequestDTOPagedResult> =>
  useQuery(
    cacheKeysUtil.getTutorRequestsByLearner(
      learnerId,
      pageNumber - 1,
      pageSize
    ),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.tutorRequestGetListTutorRequestsByLeanrerIdDetail(
          learnerId,
          {
            pageIndex: pageNumber - 1,
            pageSize: pageSize,
          }
        );
      const { data } = response;
      return data;
    },
    {
      keepPreviousData: true,
    }
  );

export const useAllTutorRequestsByTutor = (
  tutorId: string,
  pageNumber: number,
  pageSize: number
): UseQueryResult<ListTutorRequestForTutorDtoPagedResult> =>
  useQuery(
    cacheKeysUtil.getTutorRequestsByTutor(tutorId, pageNumber - 1, pageSize),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.tutorRequestGetListTutorRequestsByTutorIdDetail(
          tutorId,
          {
            pageIndex: pageNumber - 1,
            pageSize: pageSize,
          }
        );
      const { data } = response;
      return data;
    },
    {
      keepPreviousData: true,
    }
  );

export const useCreateTutorRequest = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: TutorRequestDTO) =>
      tConnectService.api.tutorRequestCreateTutorRequestCreate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient
            .invalidateQueries
            // cacheKeysUtil.tutorLearnerSubjectDetail(tutorLearnerSubjectId)
            (),
        ]);
      },
    }
  );
};

export const useUpdateTutorRequest = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: TutorRequestDTO) =>
      tConnectService.api.tutorRequestUpdateTutorRequestUpdate(data.id!, data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.getTutorRequestDetail(args.id!)
          ),
        ]);
      },
    }
  );
};

export const useListTutorRegisteredDetail = (
  tutorRequestId: number
): UseQueryResult<TutorRequestWithTutorsDTO> =>
  useQuery(
    cacheKeysUtil.getListTutorRegisteredDetail(tutorRequestId),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.tutorRequestGetListTutorRequestRegisteredDetail(
          tutorRequestId
        );
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
    }
  );

export const useTutorRequestDetail = (
  tutorRequestId: number
): UseQueryResult<TutorRequestDTO> =>
  useQuery(
    cacheKeysUtil.getTutorRequestDetail(tutorRequestId),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.tutorRequestGetTutorRequestByIdDetail(
          tutorRequestId
        );
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
    }
  );

export const useTutorLearnerSubjectDetailByTutorRequestId = (
  tutorRequestId: number
): UseQueryResult<TutorRequestDTO> =>
  useQuery(
    cacheKeysUtil.getTutorLearnerSubjectDetailByTutorRequestId(tutorRequestId),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.tutorRequestGetTutorLearnerSubjectInfoByTutorRequestIdDetail(
          tutorRequestId
        );
      const { data } = response;
      return data.data;
    }
  );

export const useInterestTutor = (tutorRequestId: number) => {
  const queryClient = useQueryClient();

  return useMutation(
    async (tutorId: string) =>
      tConnectService.api.tutorRequestSendTutorRequestEmailCreate(
        tutorRequestId,
        {
          tutorID: tutorId,
        }
      ),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.getTutorRequestDetail(tutorRequestId)
          ),
          queryClient.invalidateQueries(
            cacheKeysUtil.getListTutorRegisteredDetail(tutorRequestId)
          ),
        ]);
      },
    }
  );
};

export const useChooseTutor = (tutorRequestId: number) => {
  const queryClient = useQueryClient();
  const { user } = useAppContext();

  return useMutation(
    async (tutorId: string) =>
      tConnectService.api.tutorRequestChooseTutorForTutorRequestAsyncUpdate(
        tutorRequestId,
        {
          tutorID: tutorId,
        }
      ),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.getTutorRequestDetail(tutorRequestId)
          ),
          queryClient.invalidateQueries(
            cacheKeysUtil.getListTutorRegisteredDetail(tutorRequestId)
          ),
          queryClient.invalidateQueries(
            cacheKeysUtil.getTutorRequestsByLearner(user?.id!, 0, 5)
          ),
        ]);
      },
    }
  );
};

export const useCloseTutorRequest = () => {
  const queryClient = useQueryClient();
  const { user } = useAppContext();

  return useMutation(
    async (tutorRequestId: number) =>
      tConnectService.api.tutorRequestCloseTutorRequestCreate(tutorRequestId),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.getTutorRequestDetail(args)
          ),
          queryClient.invalidateQueries(
            cacheKeysUtil.getTutorRequestsByLearner(user?.id!, 0, 5)
          ),
        ]);
      },
    }
  );
};
