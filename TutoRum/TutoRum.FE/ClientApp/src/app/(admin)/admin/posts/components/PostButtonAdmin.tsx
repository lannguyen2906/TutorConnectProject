"use client";
import { PostForm } from "@/app/(user)/posts/add/components/PostForm";
import { useDeletePost, usePostDetail } from "@/hooks/use-posts";
import { Button, Modal, Popconfirm } from "antd";
import { Edit, Trash } from "lucide-react";
import React from "react";
import { toast } from "react-toastify";

export const AddPostButton = () => {
  const [open, setOpen] = React.useState(false);

  return (
    <>
      <Button type="primary" onClick={() => setOpen(true)}>
        Thêm bài viết mới
      </Button>
      <Modal
        width={1000}
        style={{ top: 20 }}
        title="Thêm bài viết"
        footer={null}
        open={open}
        onCancel={() => setOpen(false)}
      >
        <PostForm setOpen={setOpen} />
      </Modal>
    </>
  );
};

export const EditPostButton = ({ postId }: { postId: number }) => {
  const [open, setOpen] = React.useState(false);
  const { data } = usePostDetail(postId);

  return (
    <>
      <Button
        type="text"
        onClick={() => setOpen(true)}
        icon={<Edit size={16} />}
      />
      <Modal
        width={1000}
        style={{ top: 20 }}
        title="Sửa bài viết"
        footer={null}
        open={open}
        onCancel={() => setOpen(false)}
      >
        {data && (
          <PostForm
            isUpdate
            defaultValues={data}
            tempFileURL={data?.thumbnail || ""}
            setOpen={setOpen}
          />
        )}
      </Modal>
    </>
  );
};

export const DeletePostButton = ({ postId }: { postId: number }) => {
  const [open, setOpen] = React.useState(false);
  const { mutateAsync, isLoading } = useDeletePost();

  const handleDelete = async () => {
    try {
      const result = await mutateAsync(postId);
      if (result.status == 201) {
        toast.success("Xóa bài viết thành công");
        setOpen(false);
      }
    } catch (error) {
      console.log(error);
      toast.error("Xóa bài viết không thành công");
    }
  };

  return (
    <>
      <Popconfirm
        title="Xoá"
        onConfirm={handleDelete}
        okText="Xoá"
        cancelText="Hủy"
        okType="danger"
      >
        <Button type="text" icon={<Trash color="red" size={16} />} />
      </Popconfirm>
    </>
  );
};
