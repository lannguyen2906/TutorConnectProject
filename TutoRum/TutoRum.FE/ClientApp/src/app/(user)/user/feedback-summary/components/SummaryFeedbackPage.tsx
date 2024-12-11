"use client";
import React from "react";
import { Doughnut } from "react-chartjs-2";
import { Card, Collapse, Empty, Spin, Table } from "antd";
import {
  Chart as ChartJS,
  Tooltip,
  Legend,
  ArcElement,
  CategoryScale,
  LinearScale,
} from "chart.js";
import { useFeedbackStatistics } from "@/hooks/use-feedback";
import { useAppContext } from "@/components/provider/app-provider";

// Đăng ký các thành phần của chart.js
ChartJS.register(Tooltip, Legend, ArcElement, CategoryScale, LinearScale);

interface AnswerBreakdown {
  answerText?: string;
  value: number;
  answerCount: string;
}

interface QuestionStatistics {
  questionType: string;
  questionText?: string;
  totalAnswerCount: string;
  answerBreakdown: AnswerBreakdown[];
}

const feedbackQuestions = [
  {
    key: "punctuality",
    question: "Sự đúng giờ của gia sư trong giờ học",
    answers: [
      { value: 4, label: "Luôn đúng giờ" },
      { value: 3, label: "Phần lớn đúng giờ" },
      { value: 2, label: "Ít khi đúng giờ" },
      { value: 1, label: "Không bao giờ đúng giờ" },
    ],
  },
  {
    key: "teachingSkills",
    question: "Kỹ năng sư phạm của gia sư",
    answers: [
      { value: 4, label: "Xuất sắc" },
      { value: 3, label: "Tốt" },
      { value: 2, label: "Khá" },
      { value: 1, label: "Yếu" },
    ],
  },
  {
    key: "supportQuality",
    question: "Mức độ hỗ trợ trong học tập của gia sư",
    answers: [
      { value: 4, label: "Rất tốt" },
      { value: 3, label: "Tốt" },
      { value: 2, label: "Khá" },
      { value: 1, label: "Cần cải thiện" },
    ],
  },
  {
    key: "responseToQuestions",
    question: "Đáp ứng yêu cầu của học viên",
    answers: [
      { value: 4, label: "Rất nhanh" },
      { value: 3, label: "Nhanh" },
      { value: 2, label: "Chậm" },
      { value: 1, label: "Rất chậm" },
    ],
  },
  {
    key: "satisfaction",
    question: "Mức độ hài lòng của học viên với khóa học",
    answers: [
      { value: 5, label: "Rất hài lòng" },
      { value: 4, label: "Hài lòng" },
      { value: 3, label: "Bình thường" },
      { value: 2, label: "Chưa hài lòng" },
      { value: 1, label: "Không hài lòng" },
    ],
  },
];

const SummaryFeedbackPage = () => {
  const { user } = useAppContext();
  const { data, isLoading } = useFeedbackStatistics(user?.id!);

  if (isLoading) return <Spin size="large" />;

  const statistic: QuestionStatistics[] =
    data?.statistics?.map((statistic) => {
      const question = feedbackQuestions.find(
        (fq) => fq.key == statistic.questionType
      );
      return {
        ...statistic,
        totalAnswerCount: statistic.totalAnswerCount || "",
        questionType: question?.key || "",
        questionText: question?.question,
        answerBreakdown: statistic.answerBreakdown
          ?.map((b) => {
            const answer = question?.answers.find((a) => a.value == b.value);
            return {
              ...b,
              answerText: answer?.label,
            };
          })
          // Sắp xếp theo điểm giảm dần
          .sort((a, b) => b.value! - a.value!),
      } as QuestionStatistics;
    }) || [];

  const collapseItems = statistic.map((s) => ({
    key: s.questionType,
    label: s.questionText,
    children: (
      <Card
        key={s.questionType}
        className="mb-6"
        title={<h3 className="text-xl font-semibold">{s?.questionText}</h3>}
        extra={
          <span className="text-sm">
            Tổng câu trả lời: {s.totalAnswerCount}
          </span>
        }
        bordered
      >
        <div className="flex justify-between items-center gap-10">
          <div className="h-full mb-6 w-1/3">
            <Doughnut
              data={{
                labels: s?.answerBreakdown.map((a) => a.answerText),
                datasets: [
                  {
                    label: "Số học viên đánh giá",
                    backgroundColor: [
                      "#44ce1b",
                      "#bbdb44",
                      "#f7e379",
                      "#f2a134",
                      "#e51f1f",
                    ],
                    data: s.answerBreakdown.map((b) => b.answerCount),
                  },
                ],
              }}
              options={{
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                  legend: {
                    position: "bottom",
                    labels: {
                      usePointStyle: true,
                      pointStyle: "circle",
                      padding: 20,
                      font: {
                        size: 10, // Kích thước font chữ của các label trong legend
                      },
                    },
                  },
                },
              }}
            />
          </div>

          <Table
            className="border-separate text-left w-2/3"
            columns={[
              {
                title: "Câu trả lời",
                dataIndex: "answerText",
                key: "answerText",
              },
              {
                title: "Số học viên đánh giá",
                dataIndex: "answerCount",
                key: "answerCount",
              },
            ]}
            dataSource={s.answerBreakdown.map((breakdown) => ({
              key: breakdown.value,
              answerText: breakdown.answerText,
              answerCount: breakdown.answerCount,
            }))}
            pagination={false}
            bordered
            size="small"
          />
        </div>
      </Card>
    ),
  }));

  collapseItems.push({
    key: "comments",
    label: "Bình luận về gia sư",
    children: (
      <div>
        <Card title="Bình luận về gia sư" className="mb-6" bordered>
          {data?.comments?.map((comment, i) => (
            <h3 key={comment} className="text-md my-2">
              {i + 1}. {comment}
            </h3>
          ))}
        </Card>
      </div>
    ),
  });

  return (
    <div className="p-6 space-y-5">
      <Collapse items={collapseItems} />
    </div>
  );
};

export default SummaryFeedbackPage;
