import React from "react";
import CustomizedBreadcrumb from "../components/Breadcrumb/CustomizedBreadcrumb";
import PostCard from "./components/PostCard";
import Sidebar from "./components/Sidebar";

const Posts = () => {
  return (
    <div className="container mt-5">
      <CustomizedBreadcrumb currentpage="Danh sách bài viết" />
      <div className="flex flex-col md:flex-row min-h-screen">
        {/* Sidebar bên trái */}
        <div className="w-full md:w-1/3 bg-white p-4 rounded-md shadow-md self-start md:sticky top-36">
          <Sidebar />
        </div>
        {/* Phần bên phải */}
        <div className="w-full md:w-2/3 bg-gray-100 p-4">
          <PostCard />
        </div>
      </div>
    </div>
  );
};

export default Posts;
