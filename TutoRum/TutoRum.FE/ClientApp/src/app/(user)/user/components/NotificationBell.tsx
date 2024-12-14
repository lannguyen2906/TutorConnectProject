import React, { useEffect, useState } from "react";
import { Badge, List, Avatar, Button, Popover, message } from "antd";
import { BellOutlined } from "@ant-design/icons";
import { tConnectService } from "@/utils/services/tConnectService";
import { NotificationDto } from "@/utils/services/Api";
import Logo from "@/components/ui/logo";
import { formatTimeNotification } from "@/utils/other/formatter";
import Link from "next/link";
import { toast } from "react-toastify";
import { useMarkAsRead } from "@/hooks/use-notifications";

const NotificationBell = () => {
  const [initLoading, setInitLoading] = useState(true);
  const [loading, setLoading] = useState(false);
  const [data, setData] = useState<NotificationDto[]>([]);
  const [pageIndex, setPageIndex] = useState(0);
  const pageSize = 7; // Số thông báo mỗi trang
  const [totalRecords, setTotalRecords] = useState(0);
  const [totalUnreadNotifications, setTotalUnreadNotifications] = useState(0);
  const { mutateAsync: markAsRead } = useMarkAsRead();
  const [open, setOpen] = useState(false);

  useEffect(() => {
    if (pageIndex >= 0) {
      loadNotifications(pageIndex);
    } else {
      setPageIndex(0);
      loadNotifications(0);
    }
  }, [pageIndex]);

  const loadNotifications = async (page: number) => {
    try {
      setLoading(true);
      const response =
        await tConnectService.api.notificationGetAllNotificationsList({
          pageIndex: page,
          pageSize,
        });
      const notifications = response.data.data?.notifications || [];
      if (page === 0) {
        setData(notifications); // Load lại thông báo khi trang đầu tiên
      } else {
        setData((prev) => [...prev, ...notifications]); // Load thêm thông báo
      }
      setTotalUnreadNotifications(
        response.data.data?.totalUnreadNotifications || 0
      );
      setTotalRecords(response.data.data?.totalRecords || 0);
      setInitLoading(false);
    } catch (error) {
      console.error(error);
      message.error("Không thể tải thông báo.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    // Kiểm tra nếu Service Worker có sẵn
    if (typeof window !== "undefined" && navigator.serviceWorker) {
      const handleNewNotification = (event: any) => {
        const message = event.data;
        const { title } = message.notification;
        // Hiển thị thông báo toast
        toast.info(title, {
          icon: <Logo hasText={false} size={20} />,
          position: "bottom-left",
        });
        // Reset notifications
        setPageIndex(-1);
      };

      navigator.serviceWorker.addEventListener(
        "message",
        handleNewNotification
      );

      // Cleanup khi component unmount
      return () => {
        navigator.serviceWorker.removeEventListener(
          "message",
          handleNewNotification
        );
      };
    }
  }, []);

  const onLoadMore = () => {
    const nextPageIndex = pageIndex + 1;
    if (data.length < totalRecords) {
      setPageIndex(nextPageIndex);
    }
  };

  // Đánh dấu 1 thông báo là đã đọc
  const handleMarkAsRead = async (id: number) => {
    try {
      await markAsRead([id]);
      // Reset notifications
      setPageIndex(-1);
      setOpen(false);
    } catch (error) {
      console.error("Lỗi khi đánh dấu thông báo là đã đọc", error);
    }
  };

  // Đánh dấu tất cả thông báo là đã đọc
  const markAllAsRead = async () => {
    const unreadIds =
      data.filter((item) => !item.isRead).map((item) => item.notificationId!) ||
      [];
    if (unreadIds.length > 0) {
      await markAsRead(unreadIds);
      // Reset notifications
      setPageIndex(-1);
      setOpen(false);
    }
  };
  const loadMore =
    !initLoading && !loading && data.length < totalRecords ? (
      <div
        style={{
          textAlign: "center",
          marginTop: 12,
          height: 32,
          lineHeight: "32px",
        }}
      >
        <Button onClick={onLoadMore}>Tải thêm</Button>
      </div>
    ) : null;

  const content = () => {
    return (
      <div
        id="scrollableDiv"
        style={{
          height: 400,
          width: 400,
          overflow: "auto",
        }}
      >
        {data.length > 0 ? (
          <List
            loading={initLoading}
            loadMore={loadMore}
            dataSource={data}
            renderItem={(item) => (
              <Link
                onClick={() => handleMarkAsRead(item.notificationId!)}
                href={item.href || "/#"}
                key={item.notificationId}
              >
                <List.Item
                  style={{ cursor: "pointer", padding: "8px" }}
                  className="hover:bg-muted transition-all ease-in-out duration-75 rounded-lg"
                >
                  <List.Item.Meta
                    avatar={
                      item.icon ? (
                        <Avatar src={item.icon} />
                      ) : (
                        <Logo hasText={false} size={25} />
                      )
                    }
                    title={
                      <div className="font-semibold flex items-center gap-2">
                        {item.title}
                      </div>
                    }
                    description={<p className="text-xs">{item.description}</p>}
                  />
                  <div className="text-xs text-Blueviolet ms-4 flex gap-2 items-center">
                    {!item.isRead && (
                      <div className="bg-Blueviolet h-[8px] w-[8px] rounded-full" />
                    )}
                    {formatTimeNotification(item.createdDate ?? "")}
                  </div>
                </List.Item>
              </Link>
            )}
          />
        ) : (
          <p style={{ textAlign: "center", padding: "16px" }}>
            Không có thông báo nào
          </p>
        )}
      </div>
    );
  };

  return (
    <Popover
      content={content}
      title={
        <div className="w-full flex justify-between">
          <p className="text-lg">Thông báo</p>
          <button
            onClick={markAllAsRead}
            className="hover:text-Blueviolet transition-all ease-in-out duration-200 text-sm font-normal underline text-muted-foreground"
          >
            Đánh dấu tất cả là đã đọc
          </button>
        </div>
      }
      trigger="click"
      style={{ padding: 0 }}
      open={open}
      onOpenChange={(newOpen: boolean) => setOpen(newOpen)}
      placement="bottomRight"
    >
      <Badge count={totalUnreadNotifications} size="default" color="Blueviolet">
        <Button
          type="default"
          shape="circle"
          icon={<BellOutlined size={16} />}
        />
      </Badge>
    </Popover>
  );
};

export default NotificationBell;
