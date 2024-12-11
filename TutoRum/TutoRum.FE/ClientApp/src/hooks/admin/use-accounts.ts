import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import {
  AddPostsDTO,
  AssignRoleAdminDto,
  Post,
  PostCategory,
  PostsDTO,
  PostsHomePageDTO,
  UpdatePostDTO,
  ViewAccount,
} from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "react-query";

export const usePostDetail = (id: number): UseQueryResult<PostsDTO> =>
  useQuery(cacheKeysUtil.postDetail(id), async (): Promise<any> => {
    const response = await tConnectService.api.postGetPostByIdList({
      postId: id,
    });
    const { data } = response;

    return data.data;
  });

export const useAccountList = (): UseQueryResult<ViewAccount[]> =>
  useQuery(cacheKeysUtil.adminAccountList(), async (): Promise<any> => {
    const response = await tConnectService.api.accountsViewAllAccountsList();
    const { data } = response;
    return data.data;
  });

export const usePostCategoryList = (): UseQueryResult<PostCategory[]> =>
  useQuery(cacheKeysUtil.postCategoryList(), async (): Promise<any> => {
    const response = await tConnectService.api.postGetAllPostCategoriesList();
    const { data } = response;
    return data.data;
  });

export const useAssignRole = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: AssignRoleAdminDto) =>
      tConnectService.api.adminAssignRoleAdminCreate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.adminAccountList()),
        ]);
      },
    }
  );
};

export const useBlockUser = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: string) => tConnectService.api.accountsBlockUserCreate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.adminAccountList()),
        ]);
      },
    }
  );
};

export const useUnblockUser = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: string) => tConnectService.api.accountsUnblockUserCreate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.adminAccountList()),
        ]);
      },
    }
  );
};
