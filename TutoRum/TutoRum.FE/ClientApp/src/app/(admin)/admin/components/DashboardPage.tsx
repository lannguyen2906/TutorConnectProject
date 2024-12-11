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
} from "lucide-react"; // Hoặc sử dụng font-awesome nếu muốn

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
  // Key metrics data
  const metrics = [
    {
      title: "Yêu cầu thanh toán chờ phê duyệt",
      value: 120,
      color: "#FF9800", // Màu cam
      icon: <CheckCircle size={16} style={{ color: "#FF9800" }} />, // Biểu tượng duyệt
    },
    {
      title: "Gia sư đăng ký mới",
      value: 200,
      color: "#4CAF50", // Màu xanh lá
      icon: <UserPlus size={16} style={{ color: "#4CAF50" }} />, // Biểu tượng người dùng mới
    },
    {
      title: "Học sinh đăng ký mới",
      value: 150,
      color: "#2196F3", // Màu xanh dương
      icon: <Users size={16} style={{ color: "#2196F3" }} />, // Biểu tượng học sinh
    },
    {
      title: "Tổng số lớp học hiện tại",
      value: 50,
      color: "#9C27B0", // Màu tím
      icon: <BookOpen size={16} style={{ color: "#9C27B0" }} />, // Biểu tượng lớp học
    },
    {
      title: "Yêu cầu chưa giải quyết",
      value: 10,
      color: "#F44336", // Màu đỏ
      icon: <AlertTriangle size={16} style={{ color: "#F44336" }} />, // Biểu tượng cảnh báo
    },
  ];

  // Biểu đồ Doanh thu (Bar Chart)
  const revenueData = {
    labels: ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4"],
    datasets: [
      {
        label: "Doanh thu",
        data: [500, 1000, 800, 1200],
        backgroundColor: "rgba(75, 192, 192, 0.2)",
        borderColor: "rgba(75, 192, 192, 1)",
        borderWidth: 1,
      },
    ],
  };

  // Biểu đồ phân bố gia sư theo thành phố (Bar Chart)
  const cityDistributionData = {
    labels: ["Hà Nội", "Hồ Chí Minh", "Đà Nẵng", "Cần Thơ"],
    datasets: [
      {
        label: "Số lượng gia sư",
        data: [50, 120, 80, 40],
        backgroundColor: "rgba(54, 162, 235, 0.2)",
        borderColor: "rgba(54, 162, 235, 1)",
        borderWidth: 1,
      },
    ],
  };

  // Biểu đồ phân bố gia sư theo môn học (Pie Chart)
  const subjectDistributionData = {
    labels: ["Toán", "Lý", "Hóa", "Anh văn", "Lịch sử"],
    datasets: [
      {
        label: "Số lượng gia sư",
        data: [100, 80, 70, 60, 50],
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
    labels: ["Hà Nội", "Hồ Chí Minh", "Đà Nẵng", "Cần Thơ"],
    datasets: [
      {
        label: "Số lượng yêu cầu gia sư",
        data: [30, 50, 40, 20],
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

      <Row gutter={16}>
        <Col span={12}>
          <Card title="Doanh thu" bordered={false}>
            <Line data={revenueData} options={{ responsive: true }} />
          </Card>
        </Col>

        <Col span={12}>
          <Card title="Phân bố gia sư theo thành phố" bordered={false}>
            <Bar data={cityDistributionData} options={{ responsive: true }} />
          </Card>
        </Col>
      </Row>

      <Row gutter={16} style={{ marginTop: 20 }}>
        <Col span={12}>
          <Card title="Số lượng yêu cầu gia sư theo thành phố" bordered={false}>
            <Line data={cityRequestData} options={{ responsive: true }} />
          </Card>
        </Col>

        <Col span={12}>
          <Card title="Đánh giá gia sư theo trình độ" bordered={false}>
            <Bar
              data={qualificationRatingData}
              options={{ responsive: true }}
            />
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
