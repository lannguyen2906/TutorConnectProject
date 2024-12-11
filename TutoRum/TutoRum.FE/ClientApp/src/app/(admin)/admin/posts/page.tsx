import React from "react";
import PostsTableAdmin from "./components/PostAdminTable";
import { ContentLayout } from "../../components/content-layout";
import PostAdminTable from "./components/PostAdminTable";

const page = () => {
  return (
    <ContentLayout title="Danh sách bài viết">
      <PostAdminTable />
    </ContentLayout>
  );
};

export default page;
