import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import {
  CreateFeedbackDto,
  FeedbackDetail,
  FeedbackDto,
  FeedbackStatisticsResponse,
} from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "react-query";

export const useFeedbackDetailByTutorLearnerSubjectId = (
  tutorLearnerSubjectId: number
): UseQueryResult<FeedbackDto> =>
  useQuery(
    cacheKeysUtil.feedbackDetailByTutorLearnerSubjectId(tutorLearnerSubjectId),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.feedbackGetByTutorLearnerSubjectIdDetail(
          tutorLearnerSubjectId
        );
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
      enabled: !!tutorLearnerSubjectId,
    }
  );

export const useFeedbackDetailByTutorId = (
  tutorId: string,
  showAll: boolean
): UseQueryResult<FeedbackDetail> =>
  useQuery(
    cacheKeysUtil.feedbackDetailByTutorId(tutorId, showAll),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.feedbackGetFeedbackDetailByTutorIdDetail(
          tutorId,
          { showAll }
        );
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
      enabled: !!tutorId,
    }
  );

export const useCreateFeedback = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: CreateFeedbackDto) =>
      tConnectService.api.feedbackCreateFeedbackCreate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.feedbackDetailByTutorLearnerSubjectId(
              args.tutorLearnerSubjectId!
            )
          ),
        ]);
      },
    }
  );
};

export const useUpdateFeedback = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: FeedbackDto) =>
      tConnectService.api.feedbackUpdateFeedbackUpdate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.feedbackDetailByTutorLearnerSubjectId(
              args.tutorLearnerSubjectId!
            )
          ),
        ]);
      },
    }
  );
};

export const useFeedbackStatistics = (
  tutorId: string
): UseQueryResult<FeedbackStatisticsResponse> =>
  useQuery(
    cacheKeysUtil.feedbackStatistics(tutorId),
    async (): Promise<any> => {
      const response = await tConnectService.api.feedbackGetStatisticsList({
        tutorId,
      });
      const { data } = response;
      return data.data;
    },
    {
      enabled: !!tutorId,
    }
  );
