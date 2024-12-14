/* eslint-disable react-hooks/exhaustive-deps */
"use client";

import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { useAppContext } from "@/components/provider/app-provider";
import MDEditor from "@uiw/react-md-editor";
import { Progress } from "@/components/ui/progress";
import Image from "next/image";
import { Edit, Loader2 } from "lucide-react";
import { Button, Select } from "antd";
import { toast } from "react-toastify";

import { PostSchema, PostType } from "@/utils/schemaValidations/post.schema";
import {
  AddPostsDTO,
  Post,
  PostsDTO,
  UpdatePostDTO,
} from "@/utils/services/Api";
import {
  useCreatePost,
  usePostCategoryList,
  useUpdatePost,
} from "@/hooks/use-posts";
import useTempFileUpload from "@/hooks/use-upload-file";
import PostDetail from "../../[id]/components/PostDetail";
import { mockPost } from "@/app/(user)/tutors/[id]/components/mockData";
import useUploadFileEasy from "@/hooks/use-upload-file-easy";
import UploadThumbnail from "./UploadThumbnail";
import { getFilesByUrl } from "@/hooks/use-list-files";

interface Props {
  defaultValues?: PostsDTO;
  tempFileURL?: string;
  isUpdate?: boolean;
  setOpen?: (open: boolean) => void;
}

export const PostForm = ({
  defaultValues,
  tempFileURL,
  isUpdate,
  setOpen,
}: Props) => {
  // States
  const [previewMode, setPreviewMode] = useState(false);
  const [previewPostData, setPreviewPostData] = useState<Post>(
    defaultValues || { thumbnail: mockPost.thumbnailUrl }
  );

  const [tempUrl, setTempUrl] = useState<string | null>(null);
  const { uploadFiles, handleFileChange } = useUploadFileEasy();
  const { data: categoryData } = usePostCategoryList();
  const { mutateAsync: createPost, isLoading: createPostLoading } =
    useCreatePost();
  const { mutateAsync: updatePost, isLoading: updatePostLoading } =
    useUpdatePost();

  // Form setup
  const form = useForm<PostType>({
    resolver: zodResolver(PostSchema),
    defaultValues: defaultValues
      ? {
          content: defaultValues.content || "",
          subContent: defaultValues.subContent || "",
          title: defaultValues.title || "",
          postType: defaultValues.postType?.toString() || "",
        }
      : undefined,
  });

  useEffect(() => {
    const fetchData = async () => {
      const loadedData = await getFilesByUrl(tempFileURL || "");
      if (loadedData && loadedData.length > 0) {
        setTempUrl(loadedData.at(0) || "");
      }
    };

    fetchData();
  }, []);

  useEffect(() => {
    if (defaultValues) {
      form.reset({
        content: defaultValues?.content || "",
        subContent: defaultValues?.subContent || "",
        title: defaultValues?.title || "",
        postType: defaultValues?.postType?.toString() || "",
      });
    }
  }, []);

  // Watch form changes for preview
  useEffect(() => {
    form.watch((values) => {
      setPreviewPostData({
        content: values.content,
        createdDate: new Date().toISOString(),
        postCategory: categoryData?.find(
          (item) => item.postType?.toString() === values.postType
        ),
        title: values.title,
        thumbnail: tempUrl || mockPost.thumbnailUrl,
        subcontent: values.subContent,
      });
    });
  }, [form, categoryData, tempUrl]);

  // Submit handler
  const onSubmit = async (values: PostType) => {
    try {
      const url = await uploadFiles("posts");
      const postData: AddPostsDTO = {
        content: values.content,
        title: values.title,
        subContent: values.subContent,
        thumbnail: url[0] || tempFileURL,
        postType: Number.parseInt(values.postType),
      };

      const response = isUpdate
        ? await updatePost({ postId: defaultValues?.postId, ...postData })
        : await createPost(postData);

      if (response.data.status === 200) {
        toast.success(`${isUpdate ? "Cập nhật" : "Tạo"} bài viết thành công`);
        setOpen && setOpen(false);
      }
    } catch (err) {
      toast.error("Xử lý không thành công");
      console.error(err);
    }
  };

  // UI rendering
  return (
    <>
      {!previewMode ? (
        <Form {...form}>
          <form className="space-y-8" onSubmit={form.handleSubmit(onSubmit)}>
            <h2 className="text-center text-3xl font-bold">Tạo bài viết mới</h2>
            {/* Tiêu đề */}
            <FormField
              control={form.control}
              name="title"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Tiêu đề</FormLabel>
                  <FormControl>
                    <Input placeholder="Cầu nối gia sư..." {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            {/* Thể loại */}
            <FormField
              control={form.control}
              name="postType"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Thể loại</FormLabel>
                  <Select
                    className="w-full"
                    {...field}
                    options={categoryData?.map((c) => ({
                      value: c.postType?.toString(),
                      label: c.postName,
                    }))}
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
            {/* Thumbnail */}
            <UploadThumbnail
              handleFileChange={handleFileChange}
              tempUrl={tempUrl || ""}
            />

            {/* Mô tả */}
            <FormField
              control={form.control}
              name="subContent"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Mô tả</FormLabel>
                  <FormControl>
                    <Input placeholder="Mô tả ngắn..." {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            {/* Nội dung */}
            <FormField
              control={form.control}
              name="content"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Nội dung</FormLabel>
                  <div data-color-mode="light">
                    <MDEditor value={field.value} onChange={field.onChange} />
                  </div>
                  <FormMessage />
                </FormItem>
              )}
            />
            {/* Actions */}
            <div className="flex items-center justify-between">
              <Button
                type="dashed"
                onClick={() => setPreviewMode(true)}
                className="fixed bottom-5 left-5"
              >
                Xem thử bài mẫu
              </Button>
              <Button
                type="primary"
                htmlType="submit"
                loading={createPostLoading || updatePostLoading}
              >
                {isUpdate ? "Cập nhật bài viết" : "Tạo bài viết"}
              </Button>
            </div>
          </form>
        </Form>
      ) : (
        <div>
          <PostDetail {...previewPostData} />
          <Button type="dashed" onClick={() => setPreviewMode(false)}>
            Quay lại chỉnh sửa
          </Button>
        </div>
      )}
    </>
  );
};
