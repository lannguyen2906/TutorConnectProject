import { formatDate } from "date-fns";
import { vi } from "date-fns/locale";
import dayjs from "dayjs";

export const formatDateVi = (date: Date) =>
  formatDate(date, "dd/MM/yyyy", { locale: vi });

export const formatNumber = (value: string) =>
  `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ",");

export const formatTimeNotification = (date: string) => {
  const now = dayjs();
  const inputDate = dayjs(date);

  const diffInMinutes = now.diff(inputDate, "minute");
  const diffInHours = now.diff(inputDate, "hour");
  const diffInDays = now.diff(inputDate, "day");
  const diffInWeeks = now.diff(inputDate, "week");

  if (diffInMinutes < 60) {
    return `${diffInMinutes} phút trước`;
  } else if (diffInHours < 24) {
    return `${diffInHours} giờ trước`;
  } else if (diffInDays < 7) {
    return `${diffInDays} ngày trước`;
  } else {
    return `${diffInWeeks} tuần trước`;
  }
};
