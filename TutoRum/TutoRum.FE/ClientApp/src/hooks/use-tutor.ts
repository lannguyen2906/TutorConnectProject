import { useAppContext } from "@/components/provider/app-provider";
import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import {
  AddScheduleDTO,
  AddTutorDTO,
  DeleteScheduleDTO,
  MajorMinorDto,
  QualificationLevelDto,
  RegisterLearnerDTO,
  ScheduleGroupDTO,
  ScheduleViewDTO,
  Tutor,
  TutorDto,
  TutorFilterDto,
  TutorHomePageDTO,
  TutorSummaryDto,
  UpdateScheduleDTO,
  UpdateTutorInforDTO,
} from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "react-query";

export const useTutorHomePage = (
  filter: TutorFilterDto,
  pageNumber: number,
  pageSize: number
): UseQueryResult<TutorHomePageDTO> =>
  useQuery(
    [cacheKeysUtil.tutorList(), pageNumber, pageSize],
    async (): Promise<any> => {
      const response = await tConnectService.api.tutorTutorHomePageCreate(
        filter,
        {
          index: pageNumber - 1,
          size: pageSize,
        }
      );
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
    }
  );

export const useRegisterTutor = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: AddTutorDTO) =>
      tConnectService.api.tutorRegisterTutorCreate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.currentProfile()),
        ]);
      },
    }
  );
};

export const useUpdateTutor = () => {
  const queryClient = useQueryClient();
  const { user } = useAppContext();

  return useMutation(
    async (data: UpdateTutorInforDTO) =>
      tConnectService.api.tutorUpdateTutorInfoUpdate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.tutorDetail(user?.id!)),
        ]);
      },
    }
  );
};

export const useDeleteTeachingLocation = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async ({
      locationIds,
      tutorId,
    }: {
      locationIds: number[];
      tutorId: string;
    }) =>
      tConnectService.api.tutorDeleteTeachingLocationDelete(locationIds, {
        tutorId,
      }),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.tutorDetail(args.tutorId)
          ),
        ]);
      },
    }
  );
};

export const useDeleteCertificate = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async ({ certId, tutorId }: { certId: number; tutorId: string }) =>
      tConnectService.api.tutorDeleteCertificateAsyncDelete(certId, {
        tutorId,
      }),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.tutorDetail(args.tutorId)
          ),
        ]);
      },
    }
  );
};

export const useDeleteTutorSubject = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async ({
      tutorSubjectId,
      tutorId,
    }: {
      tutorSubjectId: number;
      tutorId: string;
    }) =>
      tConnectService.api.tutorDeleteTutorSubjectAsyncDelete(tutorSubjectId, {
        tutorId,
      }),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.tutorDetail(args.tutorId)
          ),
        ]);
      },
    }
  );
};

export const useMajorsAndSpecializations = (): UseQueryResult<
  MajorMinorDto[]
> =>
  useQuery(cacheKeysUtil.majorsAndSpecializations(), async (): Promise<any> => {
    const response = await tConnectService.api.tutorMajorsWithMinorList();
    const { data } = response;
    return data.data;
  });

export const useQualificationLevelList = (): UseQueryResult<
  QualificationLevelDto[]
> =>
  useQuery(cacheKeysUtil.qualificationLevelList(), async (): Promise<any> => {
    const response =
      await tConnectService.api.qualificationLevelGetAllQualificationLevelsList();
    const { data } = response;
    return data.data;
  });

export const useTutorDetail = (tutorId: string): UseQueryResult<TutorDto> =>
  useQuery(
    cacheKeysUtil.tutorDetail(tutorId),
    async (): Promise<any> => {
      const response = await tConnectService.api.tutorGetTutorByIdDetail(
        tutorId
      );
      const { data } = response;
      return data.data;
    },
    {
      enabled: !!tutorId,
    }
  );

export const useRegisterTutorSubject = (tutorId: string) => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: RegisterLearnerDTO) =>
      tConnectService.api.tutorLearnerSubjectRegisterLearnerCreate(data, {
        tutorId,
      }),
    {
      onSuccess: async () => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.currentProfile()),
        ]);
      },
    }
  );
};

export const useTutorSchedule = (
  tutorId: string
): UseQueryResult<ScheduleGroupDTO[]> =>
  useQuery(
    cacheKeysUtil.tutorSchedule(tutorId),
    async (): Promise<any> => {
      const response = await tConnectService.api.scheduleDetail(tutorId);
      const { data } = response;
      return data.data;
    },
    {
      enabled: !!tutorId,
    }
  );

export const useTopTutor = (size: number): UseQueryResult<TutorSummaryDto[]> =>
  useQuery(
    cacheKeysUtil.topTutor(size),
    async (): Promise<any> => {
      const response = await tConnectService.api.tutorGetTopTutorDetail(size);
      const { data } = response;
      return data.data;
    },
    {
      enabled: !!size,
    }
  );

export const useUpdateSchedule = (tutorId: string) => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: UpdateScheduleDTO) =>
      tConnectService.api.scheduleUpdateUpdate(tutorId, data),
    {
      onSuccess: async () => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.tutorSchedule(tutorId)),
        ]);
      },
    }
  );
};

export const useAddSchedule = (tutorId: string) => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: AddScheduleDTO) =>
      tConnectService.api.scheduleAddCreate(tutorId, data),
    {
      onSuccess: async () => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.tutorSchedule(tutorId)),
        ]);
      },
    }
  );
};

export const useDeleteSchedule = (tutorId: string) => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: DeleteScheduleDTO) =>
      tConnectService.api.scheduleDeleteDelete(tutorId, data),
    {
      onSuccess: async () => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.tutorSchedule(tutorId)),
        ]);
      },
    }
  );
};
