import { Bar } from "react-chartjs-2";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
} from "chart.js";

ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
);

const revenueData = {
  labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun"], // Các tháng
  datasets: [
    {
      label: "Doanh thu (5% trên mỗi hóa đơn)",
      data: [500, 600, 700, 900, 1000, 1200], // Doanh thu thu được theo tháng
      backgroundColor: "rgba(75, 192, 192, 0.5)",
      borderColor: "rgba(75, 192, 192, 1)",
      borderWidth: 1,
    },
  ],
};

const revenueOptions = {
  responsive: true,
  plugins: {
    title: {
      display: true,
      text: "Doanh thu theo tháng",
    },
  },
  scales: {
    x: {
      title: {
        display: true,
        text: "Tháng",
      },
    },
    y: {
      title: {
        display: true,
        text: "Doanh thu (VND)",
      },
    },
  },
};

const RevenueChart = () => {
  return <Bar data={revenueData} options={revenueOptions} />;
};

export default RevenueChart;
