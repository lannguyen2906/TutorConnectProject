"use client";
import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import { Form } from "@/components/ui/form";
import { Button, Drawer, Pagination, Skeleton } from "antd";
import CustomizedBreadcrumb from "../../components/Breadcrumb/CustomizedBreadcrumb";
import TutorRequestFilters from "./TutorRequestFilters";
import TutorRequestList from "./TutorRequestList";
import TutorRequestSideInfo from "./TutorRequestSideInfo";
import { useSearchParams } from "next/navigation";
import { Filter } from "lucide-react";
import { useAllTutorRequests } from "@/hooks/use-tutor-request";
import Link from "next/link";

const TutorRequestListPage = () => {
  const form = useForm();
  const search = useSearchParams();
  const [filterDrawerOpen, setFilterDrawerOpen] = React.useState(false);
  const [pageIndex, setPageIndex] = React.useState(1);
  const [pageSize, setPageSize] = React.useState(10);

  const priceRange = form.watch("priceRange");

  const { data, isLoading } = useAllTutorRequests(
    pageIndex,
    pageSize,
    form.watch("search"),
    form.watch("city"),
    form.watch("district"),
    priceRange && priceRange.length === 2 && priceRange[0],
    priceRange && priceRange.length === 2 && priceRange[1],
    undefined,
    form.watch("subjects"),
    form.watch("qualification"),
    form.watch("tutorGender")
  );

  useEffect(() => {
    if (search) {
      // Chuyển đổi search params thành đối tượng và lọc bỏ các giá trị rỗng
      const defaultValue = {
        subjects: search.getAll("subjects").filter((val) => val),
        priceRange:
          search.getAll("priceRange").length > 0
            ? search.getAll("priceRange").map(Number)
            : [0, 1000000], // Giá trị mặc định
        page: search.get("page") || undefined,
        city: search.get("city") || undefined,
        districts: search.getAll("districts") || undefined,
        schedule: search.get("schedule") || undefined,
        sort: search.get("sort") || undefined,
      };

      // Loại bỏ các giá trị không có (null, undefined hoặc mảng rỗng)
      const filteredDefaultValue = Object.fromEntries(
        Object.entries(defaultValue).filter(
          ([, value]) =>
            value !== undefined &&
            value !== null &&
            value !== "" &&
            !(Array.isArray(value) && value.length === 0)
        )
      );

      form.reset(filteredDefaultValue);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (isLoading) {
    return <Skeleton />;
  }

  return (
    <div className="w-full container mt-5">
      <CustomizedBreadcrumb currentpage="Danh sách yêu cầu" />
      <div className="text-2xl font-bold border-b-2 w-fit mb-5">
        Danh sách yêu cầu tìm gia sư
      </div>
      <div className="flex justify-end mb-5 xl:hidden">
        <Button
          type="primary"
          icon={<Filter />}
          onClick={() => setFilterDrawerOpen(true)}
        >
          Bộ lọc
        </Button>
      </div>
      <Form {...form}>
        <div className="hidden xl:block">
          <TutorRequestFilters />
        </div>
        <Drawer
          open={filterDrawerOpen}
          onClose={() => setFilterDrawerOpen(false)}
          placement="right"
          title="Bộ lọc"
        >
          <TutorRequestFilters />
        </Drawer>
        <div className="flex flex-col xl:flex-row gap-10">
          <div className="w-full xl:w-3/4">
            <TutorRequestList data={data?.items ?? []} />
            <div className="mt-5 w-full">
              <Pagination
                defaultCurrent={1}
                total={data?.totalRecords}
                onChange={(page) => setPageIndex(page)}
                current={pageIndex}
              />
            </div>
          </div>
          <div className="w-full xl:w-1/4 space-y-5">
            <Link href={"/tutor-requests/add"}>
              <Button className="w-full" size="large" type="primary">
                Tạo mới yêu cầu ngay
              </Button>
            </Link>
            <TutorRequestSideInfo />
          </div>
        </div>
      </Form>
    </div>
  );
};

export default TutorRequestListPage;
