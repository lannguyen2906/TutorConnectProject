"use client";
import CustomizedBreadcrumb from "../../components/Breadcrumb/CustomizedBreadcrumb";
import { PostForm } from "./components/PostForm";

export default function CreatePost() {
  return (
    <div className="container mt-5">
      <CustomizedBreadcrumb currentpage="Thêm bài viết mới" />
      <PostForm />
    </div>
  );
}
