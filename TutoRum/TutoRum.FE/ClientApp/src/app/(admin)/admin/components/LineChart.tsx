import React from "react";
import { Line } from "react-chartjs-2";
import {
  Chart as ChartJS,
  LineElement,
  CategoryScale,
  LinearScale,
  PointElement,
  Tooltip,
  Legend,
  ChartOptions,
} from "chart.js";

// Đăng ký các thành phần cần thiết
ChartJS.register(
  LineElement,
  CategoryScale,
  LinearScale,
  PointElement,
  Tooltip,
  Legend
);

const LineChart = () => {
  // Dữ liệu biểu đồ
  const data = {
    labels: ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5"], // Trục X
    datasets: [
      {
        label: "Gia sư mới",
        data: [10, 20, 30, 25, 35], // Dữ liệu trục Y
        borderColor: "rgba(75,192,192,1)", // Màu viền
        backgroundColor: "rgba(75,192,192,0.2)", // Màu nền
        tension: 0.4, // Độ cong của đường
      },
      {
        label: "Học sinh mới",
        data: [15, 25, 20, 30, 40],
        borderColor: "rgba(153,102,255,1)",
        backgroundColor: "rgba(153,102,255,0.2)",
        tension: 0.4,
      },
    ],
  };

  // Cấu hình tùy chọn biểu đồ
  const options: ChartOptions<"line"> = {
    responsive: true,
    plugins: {
      title: {
        display: true,
        text: "Thống kê gia sư và học sinh",
      },
      legend: {
        display: true, // Hiển thị chú thích
        position: "top", // Vị trí của chú thích
      },
    },
    scales: {
      x: {
        title: {
          display: true,
          text: "Tháng", // Tên trục X
        },
      },
      y: {
        title: {
          display: true,
          text: "Số lượng", // Tên trục Y
        },
        min: 0, // Giá trị nhỏ nhất trên trục Y
        max: 50, // Giá trị lớn nhất trên trục Y
      },
    },
  };

  return <Line data={data} options={options} />;
};

export default LineChart;
