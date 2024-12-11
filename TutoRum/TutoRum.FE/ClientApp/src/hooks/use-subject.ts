import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import { Subject, SubjectDTO, SubjectFilterDTO } from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "react-query";

export const useSubjectList = (): UseQueryResult<SubjectFilterDTO[]> =>
  useQuery(cacheKeysUtil.subjectList(), async (): Promise<any> => {
    const response = await tConnectService.api.subjectGetAllSubjectsList();
    const { data } = response;
    return data.data;
  });

export const useTopSubject = (
  size: number
): UseQueryResult<SubjectFilterDTO[]> =>
  useQuery(cacheKeysUtil.topSubject(size), async (): Promise<any> => {
    const response = await tConnectService.api.subjectGetTopSubjectDetail(size);
    const { data } = response;
    return data.data;
  });

export const useCreateSubject = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: SubjectDTO) =>
      tConnectService.api.subjectCreateSubjectForSuperAdminCreate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.subjectList()),
        ]);
      },
    }
  );
};

export const useUpdateSubject = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: SubjectDTO) =>
      tConnectService.api.subjectUpdateSubjectForSuperAdminUpdate(
        data.subjectId!,
        data
      ),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.subjectList()),
        ]);
      },
    }
  );
};

export const useDeleteSubject = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (subjectId: number) =>
      tConnectService.api.subjectDeleteSubjectForSuperAdminDelete(subjectId),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.subjectList()),
        ]);
      },
    }
  );
};
