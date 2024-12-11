import { Button, Result } from "antd";
import Link from "next/link";
import React from "react";

const page = () => {
  return (
    <Result
      status="403"
      title="403"
      subTitle="Xin lỗi bạn không có quyền truy cập trang này"
      extra={
        <Link href="/home">
          <Button type="primary">Trang chủ</Button>
        </Link>
      }
    />
  );
};

export default page;
