"use client";
import { useSearchParams } from "next/navigation";
import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import SearchBar from "./SearchBar";
import TutorList from "./TutorList";
import TopTutor from "./TopTutor";
import TopSubject from "./TopSubject";
import { Form } from "@/components/ui/form";
import {
  TutorFilterSchema,
  TutorFilterType,
} from "@/utils/schemaValidations/tutor.schema";
import { zodResolver } from "@hookform/resolvers/zod";

const TutorListPage = () => {
  const form = useForm<TutorFilterType>({
    resolver: zodResolver(TutorFilterSchema),
    defaultValues: {
      subjects: [], // Khởi tạo giá trị mặc định là một mảng rỗng
    },
  });
  const search = useSearchParams();
  const [filterDrawerOpen, setFilterDrawerOpen] = React.useState(false);

  useEffect(() => {
    if (search) {
      // Chuyển đổi search params thành đối tượng và lọc bỏ các giá trị rỗng
      const defaultValue = {
        subjects: search.getAll("subjects").filter((val) => val) || undefined,
        page: search.get("page") || undefined,
        limit: search.get("limit") || undefined,
        city: search.get("city") || undefined,
        district: search.get("district") || undefined,
        schedule: search.get("schedule") || undefined,
        minPrice: search.get("minPrice") || undefined,
        maxPrice: search.get("maxPrice") || undefined,
        sort: search.get("sort") || undefined,
        searchingQuery: search.get("searchingQuery") || undefined,
      };

      // Loại bỏ các giá trị không có (null, undefined hoặc mảng rỗng)
      const filteredDefaultValue = Object.fromEntries(
        Object.entries(defaultValue).filter(
          ([, value]) => value !== undefined && value !== null && value !== ""
        )
      );

      form.reset(filteredDefaultValue);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);
  return (
    <div className="mt-5">
      {/* SearchBar at the top */}
      <Form {...form}>
        <div className="mb-4 border border-gray-300 rounded p-4">
          <SearchBar />
        </div>

        {/* Grid layout for TutorList and the right section */}
        <div className="grid grid-cols-12 gap-4">
          {/* TutorList takes 8 columns */}
          <div className="col-span-12 lg:col-span-9">
            <TutorList />
          </div>

          {/* TopTutor and TopSubject take 4 columns on large screens */}
          <div className="col-span-12 lg:col-span-3 space-y-4 ml-2">
            <TopTutor />
            <TopSubject form={form} />
          </div>
        </div>
      </Form>
    </div>
  );
};

export default TutorListPage;
