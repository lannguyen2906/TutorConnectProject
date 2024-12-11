"use client";

import { useProfileCurrent } from "@/hooks/use-auth";
import { useSaveFCMToken } from "@/hooks/use-notifications";
import { FullProfile } from "@/utils/schemaValidations/account.schema";
import { UserTokenDto } from "@/utils/services/Api";
import {
  getFcmToken,
  requestNotificationPermission,
} from "@/utils/services/firebase";
import { createContext, useContext, useEffect } from "react";

type User = FullProfile;

const AppContext = createContext<{
  user: User | null;
  remove: () => void;
  refetch: () => void;
}>({
  user: null,
  remove: () => {},
  refetch: () => {},
});

export const useAppContext = () => {
  const context = useContext(AppContext);
  return context;
};

export default function AppProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const { data: user, refetch, remove } = useProfileCurrent();
  const { mutateAsync } = useSaveFCMToken();

  useEffect(() => {
    const setupNotifications = async () => {
      try {
        // Yêu cầu quyền thông báo khi ứng dụng khởi chạy
        requestNotificationPermission();

        // Lấy FCM Token
        const token = await getFcmToken();
        if (token && user?.id) {
          console.log("FCM Token:", token);

          // Gửi token về backend
          const request: UserTokenDto = {
            deviceType: "web",
            token,
            userId: user.id,
          };
          await mutateAsync(request);
        }
      } catch (error) {
        console.error("Lỗi khi xử lý thông báo:", error);
      }
    };

    setupNotifications();
  }, [user, mutateAsync]);

  return (
    <AppContext.Provider
      value={{
        user: user || null,
        remove,
        refetch,
      }}
    >
      {children}
    </AppContext.Provider>
  );
}
