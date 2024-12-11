"use client";
import React from "react";
import {
  Avatar,
  Button,
  Card,
  Image,
  Spin,
  Table,
  Tag,
  Typography,
} from "antd";
import { useTutorDetail } from "@/hooks/use-tutor";
import { useSearchParams } from "next/navigation";
import { TeachingLocationViewDTO, TutorSubjectDto } from "@/utils/services/Api";
import { formatNumber } from "@/utils/other/formatter";
import OpenAI from "openai";
import { messages } from "../../components/AiMessage";
import MarkdownRenderer from "@/components/markdown/MarkdownRenderer";
import { cn } from "@/lib/utils";

const { Text } = Typography;

const Page = () => {
  const searchParams = useSearchParams();
  const tutor1 = searchParams.get("tutor1");
  const tutor2 = searchParams.get("tutor2");
  const [loading, setLoading] = React.useState(false);
  const [aiResponse, setAiResponse] = React.useState("");

  const { data: tutorDetail1, isLoading: isLoading1 } = useTutorDetail(
    tutor1 as string
  );
  const { data: tutorDetail2, isLoading: isLoading2 } = useTutorDetail(
    tutor2 as string
  );

  if (isLoading1 || isLoading2) {
    return <Spin tip="Đang tải thông tin gia sư..." />;
  }

  const handleClick = async () => {
    setLoading(true);
    try {
      const response = await client.chat.completions.create({
        messages: [
          ...messages,
          {
            role: "user",
            content: `Bạn hãy giúp tôi so sánh tổng quát về 2 gia sư này ${JSON.stringify(
              summarizeTutor(tutorDetail1)
            )} và ${JSON.stringify(summarizeTutor(tutorDetail2))}.`,
          },
        ],
        model: "gpt-4o",
        temperature: 0.7,
        max_tokens: 1000,
        top_p: 1,
      });

      if (response.choices.length > 0) {
        setAiResponse(
          response.choices?.at(0)?.message.content ?? "Không có phản hồi."
        );
      }
    } catch (error) {
      setAiResponse("Có lỗi xảy ra! Vui lòng thử lại sau.");
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const client = new OpenAI({
    baseURL: "https://models.inference.ai.azure.com",
    apiKey: "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",
    dangerouslyAllowBrowser: true,
  });

  const calculateAverageRate = (
    tutorSubjects: TutorSubjectDto[] | undefined | null
  ) => {
    if (!tutorSubjects?.length) return 0;
    const totalRate = tutorSubjects.reduce(
      (sum, subject) => sum + (subject.rate ?? 0),
      0
    );
    return (totalRate / tutorSubjects.length).toFixed(2);
  };

  const countDistricts = (
    teachingLocations: TeachingLocationViewDTO[] | undefined | null
  ) => {
    return (
      teachingLocations?.reduce(
        (total, location) => total + (location.districts?.length ?? 0),
        0
      ) || 0
    );
  };

  const summarizeTutor = (tutorDetail: any) => ({
    fullName: tutorDetail?.fullName || "Không có tên",
    avatarUrl: tutorDetail?.avatarUrl || "",
    briefIntroduction: tutorDetail?.briefIntroduction || "Không có thông tin",
    experience: tutorDetail?.experience || 0,
    rating: tutorDetail?.rating || 0,
    certificates: tutorDetail?.certificates?.length || 0,
    averageRate: calculateAverageRate(tutorDetail?.tutorSubjects),
    teachingLocations: countDistricts(tutorDetail?.teachingLocations),
  });
  // Define comparison data
  const dataSource = [
    {
      key: "1",
      label: "Ảnh đại diện",
      tutor1: (
        <Image
          src={tutorDetail1?.avatarUrl || ""}
          alt={tutorDetail1?.fullName || ""}
          style={{ width: "80px", height: "80px", borderRadius: "50%" }}
        />
      ),
      tutor2: (
        <Image
          src={tutorDetail2?.avatarUrl || ""}
          alt={tutorDetail2?.fullName || ""}
          style={{ width: "80px", height: "80px", borderRadius: "50%" }}
        />
      ),
    },
    {
      key: "2",
      label: "Giới thiệu",
      tutor1: tutorDetail1?.briefIntroduction || "Không có thông tin",
      tutor2: tutorDetail2?.briefIntroduction || "Không có thông tin",
    },
    {
      key: "3",
      label: "Kinh nghiệm (năm)",
      tutor1: `${tutorDetail1?.experience || 0}`,
      tutor2: `${tutorDetail2?.experience || 0}`,
    },
    {
      key: "4",
      label: "Đánh giá trung bình",
      tutor1: tutorDetail1?.rating || "Chưa có",
      tutor2: tutorDetail2?.rating || "Chưa có",
    },
    {
      key: "5",
      label: "Số lượng chứng chỉ",
      tutor1: tutorDetail1?.certificates?.length || 0,
      tutor2: tutorDetail2?.certificates?.length || 0,
    },
    {
      key: "6",
      label: "Giá dạy trung bình (VNĐ)",
      tutor1: `${formatNumber(
        calculateAverageRate(tutorDetail1?.tutorSubjects)?.toString()
      )} VNĐ`,
      tutor2: `${formatNumber(
        calculateAverageRate(tutorDetail2?.tutorSubjects)?.toString()
      )} VNĐ`,
    },
    {
      key: "7",
      label: "Số địa điểm dạy",
      tutor1: `${countDistricts(tutorDetail1?.teachingLocations)} quận/huyện`,
      tutor2: `${countDistricts(tutorDetail2?.teachingLocations)} quận/huyện`,
    },
  ];

  // Define table columns
  const columns = [
    {
      title: "Tên gia sư",
      dataIndex: "label",
      key: "label",
      render: (text: string) => <Text strong>{text}</Text>,
      width: 150, // Đặt chiều rộng cố định cho cột tiêu đề
    },
    {
      title: tutorDetail1?.fullName || "Gia sư 1",
      dataIndex: "tutor1",
      key: "tutor1",
      width: 200, // Đặt chiều rộng cố định cho cột gia sư 1
      render: (text: any, record: any) => (
        <div className={`${highlightStyle(record, "tutor1")}`}>{text}</div>
      ),
    },
    {
      title: tutorDetail2?.fullName || "Gia sư 2",
      dataIndex: "tutor2",
      key: "tutor2",
      width: 200, // Đặt chiều rộng cố định cho cột gia sư 2
      render: (text: any, record: any) => (
        <div className={`${highlightStyle(record, "tutor2")}`}>{text}</div>
      ),
    },
  ];

  const highlightStyle = (record: any, key: string) => {
    if (!record.highlight) return "";
    const otherKey = key === "tutor1" ? "tutor2" : "tutor1";
    if (record[key] > record[otherKey]) return "text-kellygreen";
    if (record[key] < record[otherKey]) return "text-red";
    return "";
  };

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold mb-6 text-center" onClick={handleClick}>
        So sánh Gia sư
      </h1>
      <Table
        dataSource={dataSource}
        columns={columns}
        pagination={false}
        bordered
      />
      <div className="mb-4 flex items-center mt-5 gap-2">
        <Avatar src="/tutoBot.png" size="large" />
        <Button type="primary" onClick={handleClick} loading={loading}>
          Bạn muốn TutoBot hỗ trợ so sánh không?
        </Button>
      </div>
      {aiResponse && (
        <div
          className={cn(
            "flex-1 bg-semiblueviolet text-midnightblue rounded-lg px-3 h-fit py-1",
            loading && "py-3"
          )}
        >
          <MarkdownRenderer content={aiResponse} />
        </div>
      )}
    </div>
  );
};

export default Page;
