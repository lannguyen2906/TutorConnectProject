import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import {
  AddPostsDTO,
  Post,
  PostCategory,
  PostsDTO,
  PostsHomePageDTO,
  UpdatePostDTO,
} from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import {
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
} from "react-query";

export const useCreatePost = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: AddPostsDTO) => tConnectService.api.postAddPostCreate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.adminPostList(0, 10)),
        ]);
      },
    }
  );
};

export const useUpdatePost = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (data: UpdatePostDTO) =>
      tConnectService.api.postUpdatePostCreate(data),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(
            cacheKeysUtil.postDetail(args.postId ?? 0)
          ),
          queryClient.invalidateQueries(cacheKeysUtil.adminPostList(0, 10)),
        ]);
      },
    }
  );
};

export const usePostDetail = (id: number): UseQueryResult<PostsDTO> =>
  useQuery(cacheKeysUtil.postDetail(id), async (): Promise<any> => {
    const response = await tConnectService.api.postGetPostByIdList({
      postId: id,
    });
    const { data } = response;

    return data.data;
  });

export const usePostsAdmin = (
  pageNumber: number,
  pageSize: number
): UseQueryResult<PostsHomePageDTO> =>
  useQuery(
    cacheKeysUtil.adminPostList(pageNumber - 1, pageSize),
    async (): Promise<any> => {
      const response = await tConnectService.api.postGetAllPostList({
        index: pageNumber - 1,
        size: pageSize,
      });
      const { data } = response;
      return data.data;
    },
    {
      keepPreviousData: true,
    }
  );

export const usePostCategoryList = (): UseQueryResult<PostCategory[]> =>
  useQuery(cacheKeysUtil.postCategoryList(), async (): Promise<any> => {
    const response = await tConnectService.api.postGetAllPostCategoriesList();
    const { data } = response;
    return data.data;
  });

export const usePostsHomepage = (
  pageNumber: number,
  pageSize: number
): UseQueryResult<PostsHomePageDTO> =>
  useQuery(
    cacheKeysUtil.postList(pageNumber - 1, pageSize),
    async (): Promise<any> => {
      const response = await tConnectService.api.postGetPostsHomePageList({
        index: pageNumber - 1,
        size: pageSize,
      });
      const { data } = response;
      return data.data;
    }
  );

export const useDeletePost = () => {
  const queryClient = useQueryClient();

  return useMutation(
    async (id: number) =>
      tConnectService.api.postDeletePostCreate({
        postID: id,
      }),
    {
      onSuccess: async (_, args) => {
        await Promise.all([
          queryClient.invalidateQueries(cacheKeysUtil.adminPostList(0, 10)),
        ]);
      },
    }
  );
};
