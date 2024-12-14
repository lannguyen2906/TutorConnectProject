import { useAppContext } from "@/components/provider/app-provider";
import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import {
  CreateClassDTO,
  HandleContractUploadDTO,
  RegisterLearnerDTO,
  SubjectDetailDto,
  TutorLearnerSubjectSummaryDetailDto,
} from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "react-query";

export const useGetSubjectDetailsList = (
  userId: string,
  viewType: string
): UseQueryResult<SubjectDetailDto[]> =>
  useQuery(
    cacheKeysUtil.subjectDetailList(userId, viewType),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.tutorLearnerSubjectGetSubjectDetailsList({
          userId,
          viewType,
        });
      const { data } = response;
      return data.data;
    },
    {
      enabled: !!userId,
    }
  );

export const useTutorLearnerSubjectDetail = (
  tutorLearnerSubjectId: number
): UseQueryResult<TutorLearnerSubjectSummaryDetailDto> =>
  useQuery(
    cacheKeysUtil.tutorLearnerSubjectDetail(tutorLearnerSubjectId),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.tutorLearnerSubjectGetTutorLearnerSubjectDetailList(
          {
            id: tutorLearnerSubjectId,
          }
        );
      const { data } = response;
      return data.data;
    },
    {
      enabled: !!tutorLearnerSubjectId,
    }
  );

export const useGetClassrooms = (
  userId: string,
  viewType: string
): UseQueryResult<SubjectDetailDto[]> =>
  useQuery(
    cacheKeysUtil.getClassrooms(userId, viewType),
    async (): Promise<any> => {
      const response =
        await tConnectService.api.tutorLearnerSubjectGetClassroomsList({
          userId,
          viewType,
        });
      const { data } = response;
      return data.data;
    },
    {
      enabled: !!userId,
    }
  );

export const useUpdateClassroom = (tutorLearnerSubjectId: number) => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: RegisterLearnerDTO) =>
      tConnectService.api.tutorLearnerSubjectUpdateClassroomUpdate(
        tutorLearnerSubjectId,
        data
      ),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.tutorLearnerSubjectDetail(tutorLearnerSubjectId)
          ),
        ]);
      },
    }
  );
};

export const useCreateClassroomByTutorRequest = (tutorRequestId: number) => {
  const queryClient = useQueryClient();
  const { user } = useAppContext();

  return useMutation(
    async (data: CreateClassDTO) =>
      tConnectService.api.tutorLearnerSubjectCreateClassFromTutorRequestCreate(
        tutorRequestId,
        data
      ),
    {
      onSuccess: async () => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.subjectDetailList(user?.id!, "viewTutors")
          ),
        ]);
      },
    }
  );
};

export const useCloseClassroom = () => {
  const queryClient = useQueryClient();
  const { user } = useAppContext();

  return useMutation(
    async (tutorLearnerSubjectId: number) =>
      tConnectService.api.tutorLearnerSubjectCloseClassUpdate(
        tutorLearnerSubjectId
      ),
    {
      onSuccess: async () => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.subjectDetailList(user?.id!, "viewTutors")
          ),
        ]);
      },
    }
  );
};

export const useUploadContract = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: HandleContractUploadDTO) =>
      tConnectService.api.tutorLearnerSubjectHandleContractUploadCreate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.tutorLearnerSubjectDetail(args.tutorLearnerSubjectId!)
          ),
        ]);
      },
    }
  );
};
