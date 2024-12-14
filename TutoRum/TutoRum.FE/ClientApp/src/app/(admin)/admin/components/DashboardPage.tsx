"use client";
import React from "react";
import { Row, Col, Statistic, Card } from "antd";
import { Line, Bar, Pie } from "react-chartjs-2";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
  ArcElement,
} from "chart.js";
import {
  CheckCircle,
  UserPlus,
  Users,
  BookOpen,
  AlertTriangle,
  DollarSign,
} from "lucide-react"; // Hoặc sử dụng font-awesome nếu muốn
import {
  useDashboardChartData,
  useDashboardKeyMetrics,
} from "@/hooks/use-dashboard";

// Đăng ký các phần tử của Chart.js
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
  ArcElement
);

const Dashboard: React.FC = () => {
  const { data: keyMetrics, isLoading: keyMetricsLoading } =
    useDashboardKeyMetrics();
  const { data: chartData, isLoading: chartDataLoading } =
    useDashboardChartData();

  // Key metrics data
  const metrics = [
    {
      title: "Tổng doanh thu hiện tại",
      value: keyMetrics?.totalRevenue || 0,
      color: "blueviolet", // Màu cam
      icon: <DollarSign size={16} style={{ color: "blueviolet" }} />, // Biểu tượng duyệt
    },
    {
      title: "Tổng gia sư",
      value: keyMetrics?.numberOfTutors || 0,
      color: "#4CAF50", // Màu xanh lá
      icon: <UserPlus size={16} style={{ color: "#4CAF50" }} />, // Biểu tượng người dùng mới
    },
    {
      title: "Tổng học sinh",
      value: keyMetrics?.numberOfLearners || 0,
      color: "#2196F3", // Màu xanh dương
      icon: <Users size={16} style={{ color: "#2196F3" }} />, // Biểu tượng học sinh
    },
    {
      title: "Tổng số lớp học hiện tại",
      value: keyMetrics?.numberOfClassrooms || 0,
      color: "#9C27B0", // Màu tím
      icon: <BookOpen size={16} style={{ color: "#9C27B0" }} />, // Biểu tượng lớp học
    },
    {
      title: "Yêu cầu thanh toán chưa giải quyết",
      value: keyMetrics?.numberOfPendingPaymentRequests || 0,
      color: "#F44336", // Màu đỏ
      icon: <AlertTriangle size={16} style={{ color: "#F44336" }} />, // Biểu tượng cảnh báo
    },
  ];

  // Biểu đồ Doanh thu (Bar Chart)
  const revenueData = {
    labels:
      chartData?.monthlyRevenues
        ?.sort((a, b) => (a.month || 0) - (b.month || 0))
        ?.map((r) => `Tháng ${r.month}`) || [],
    datasets: [
      {
        label: "Doanh thu",
        data:
          chartData?.monthlyRevenues
            ?.sort((a, b) => (a.month || 0) - (b.month || 0))
            ?.map((r) => r.revenueAmount) || [],
        backgroundColor: "rgba(75, 192, 192, 0.2)",
        borderColor: "rgba(75, 192, 192, 1)",
        borderWidth: 1,
      },
    ],
  };

  // Biểu đồ phân bố gia sư theo thành phố (Bar Chart)
  const cityDistributionData = {
    labels: chartData?.tutorsByCity?.map((c) => c.city) || [],
    datasets: [
      {
        label: "Số lượng gia sư",
        data: chartData?.tutorsByCity?.map((c) => c.tutorCount) || [],
        backgroundColor: "rgba(54, 162, 235, 0.2)",
        borderColor: "rgba(54, 162, 235, 1)",
        borderWidth: 1,
      },
    ],
  };

  // Biểu đồ phân bố gia sư theo môn học (Pie Chart)
  const subjectDistributionData = {
    labels: chartData?.tutorsBySubjects?.map((s) => s.subject) || [],
    datasets: [
      {
        label: "Số lượng gia sư",
        data: chartData?.tutorsBySubjects?.map((s) => s.tutorCount) || [],
        backgroundColor: [
          "#FF6384",
          "#36A2EB",
          "#FFCE56",
          "#FF5733",
          "#4CAF50",
        ],
      },
    ],
  };

  // Biểu đồ đánh giá gia sư theo trình độ (Bar Chart)
  const qualificationRatingData = {
    labels: ["Đại học", "Thạc sĩ", "Tiến sĩ"],
    datasets: [
      {
        label: "Đánh giá trung bình",
        data: [4.2, 4.5, 4.8],
        backgroundColor: "rgba(255, 99, 132, 0.2)",
        borderColor: "rgba(255, 99, 132, 1)",
        borderWidth: 1,
      },
    ],
  };

  // Biểu đồ số lượng yêu cầu gia sư theo thành phố (Line Chart)
  const cityRequestData = {
    labels: chartData?.learnersByCity?.map((s) => s.city) || [],
    datasets: [
      {
        label: "Số lượng học viên",
        data: chartData?.learnersByCity?.map((s) => s.learnerCount) || [],
        borderColor: "rgba(255, 99, 132, 1)",
        backgroundColor: "rgba(255, 99, 132, 0.2)",
        tension: 0.4,
      },
    ],
  };

  return (
    <Card className="shadow-lg">
      <div className="flex justify-between mb-4">
        {metrics.map((metric, index) => (
          <Card key={index}>
            <Statistic
              title={metric.title}
              value={metric.value}
              valueStyle={{ color: metric.color }}
              prefix={metric.icon}
            />
          </Card>
        ))}
      </div>

      <Card title="Doanh thu theo tháng" bordered={false}>
        <Line data={revenueData} options={{ responsive: true }} />
      </Card>

      <Row gutter={16} style={{ marginTop: 20 }}>
        <Col span={12}>
          <Card title="Phân bố học viên theo thành phố" bordered={false}>
            <Line data={cityRequestData} options={{ responsive: true }} />
          </Card>
        </Col>
        <Col span={12}>
          <Card title="Phân bố gia sư theo thành phố" bordered={false}>
            <Bar data={cityDistributionData} options={{ responsive: true }} />
          </Card>
        </Col>
      </Row>

      <Card title="Phân bố gia sư theo môn học" bordered={false}>
        <Pie
          data={subjectDistributionData}
          options={{
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
              legend: {
                position: "bottom",
              },
            },
          }}
        />
      </Card>
    </Card>
  );
};

export default Dashboard;
