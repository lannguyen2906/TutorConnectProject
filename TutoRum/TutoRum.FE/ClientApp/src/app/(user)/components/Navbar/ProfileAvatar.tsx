import { useAppContext } from "@/components/provider/app-provider";
import { AvatarFallback, AvatarImage, Avatar } from "@/components/ui/avatar";

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuPortal,
  DropdownMenuSeparator,
  DropdownMenuSub,
  DropdownMenuSubContent,
  DropdownMenuSubTrigger,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { tConnectService } from "@/utils/services/tConnectService";
import { ChevronDown, LogOut, Wallet } from "lucide-react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Modal } from "antd";
import { useState } from "react";
import { mockPost } from "../../tutors/[id]/components/mockData";
import { userDropdownLinkList } from "../../user/settings/settingLinkList";
import { formatNumber } from "@/utils/other/formatter";
import NotificationBell from "../../user/components/NotificationBell";

const ProfileAvatar = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const { user, refetch, remove } = useAppContext();
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const handleLogout = async () => {
    try {
      setLoading(true);
      await tConnectService.api.accountsSignOutCreate(); // call api logout(true);
      router.push("/login");
    } catch (error) {
      console.log(error);
    } finally {
      await fetch("/api/auth/logout", {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json",
        },
      });
      remove();
      refetch();
      setLoading(false);
      router.push("/login");
      router.refresh();
      localStorage.removeItem("sessionToken");
      localStorage.removeItem("sessionTokenExpiresAt");
    }
  };

  const isAdmin = user?.roles?.includes("admin");

  return (
    <div className="flex items-center gap-4">
      <NotificationBell />

      <Link
        className="flex items-center gap-2 xl:hidden"
        href="/user/settings/user-profile"
      >
        <Avatar className="border-2 border-slategray">
          <AvatarImage
            src={
              user?.avatarUrl?.trim()
                ? user.avatarUrl
                : mockPost.writer.avatarUrl
            }
            className="object-cover"
          />
          <AvatarFallback>{user?.fullName}</AvatarFallback>
        </Avatar>
        <span className="text-slategray">
          {user?.fullName || "Tên người dùng"}
        </span>
      </Link>
      <div className="hidden xl:block">
        <DropdownMenu>
          <DropdownMenuTrigger asChild className="cursor-pointer">
            <div className="flex items-center gap-2">
              <Avatar className="border-2 border-slategray">
                <AvatarImage
                  src={
                    user?.avatarUrl?.trim()
                      ? user.avatarUrl
                      : mockPost.writer.avatarUrl
                  }
                  className="object-cover"
                />
                <AvatarFallback>{user?.fullName}</AvatarFallback>
              </Avatar>
              <span className="text-slategray">
                {user?.fullName || "Tên người dùng"}
              </span>
              <ChevronDown className="text-slategray" size={16} />
            </div>
          </DropdownMenuTrigger>

          <DropdownMenuContent align="end" className="w-48 mt-3">
            <DropdownMenuLabel>Tài khoản</DropdownMenuLabel>
            <DropdownMenuSeparator />
            {user?.roles?.includes("tutor") && (
              <>
                <DropdownMenuItem asChild>
                  <Link href="/user/settings/wallet">
                    <Wallet className="mr-2 h-4 w-4" />
                    <span className="text-sm text-Blueviolet">
                      {formatNumber(user.balance.toString())} VND
                    </span>
                  </Link>
                </DropdownMenuItem>
                <DropdownMenuSeparator />
              </>
            )}

            <DropdownMenuGroup>
              {userDropdownLinkList(16, ["wallet"]).map(
                ({ key, label, icon, href, roles, children }) => {
                  const matchedRoles = user?.roles?.filter((role) =>
                    roles.includes(role)
                  );

                  if (matchedRoles && matchedRoles.length > 0) {
                    if (children) {
                      return (
                        <DropdownMenuSub key={key}>
                          <DropdownMenuSubTrigger className="flex items-center cursor-pointer">
                            {icon}
                            <span className="ml-2">{label}</span>
                          </DropdownMenuSubTrigger>
                          <DropdownMenuPortal>
                            <DropdownMenuSubContent>
                              {children.map((child) => (
                                <DropdownMenuItem key={child.key} asChild>
                                  <Link
                                    className="flex items-center"
                                    href={child.href}
                                  >
                                    {child.icon}
                                    <span className="ml-2">{child.label}</span>
                                  </Link>
                                </DropdownMenuItem>
                              ))}
                            </DropdownMenuSubContent>
                          </DropdownMenuPortal>
                        </DropdownMenuSub>
                      );
                    }
                    return (
                      <DropdownMenuItem key={key} asChild>
                        <Link href={href}>
                          {icon} <span className="ml-2">{label}</span>
                        </Link>
                      </DropdownMenuItem>
                    );
                  }
                }
              )}
            </DropdownMenuGroup>
            <DropdownMenuSeparator />

            <DropdownMenuItem asChild>
              <button
                className="flex items-center w-full"
                onClick={() => setIsModalOpen(true)}
              >
                <LogOut className="mr-2 h-4 w-4" />
                <span>Đăng xuất</span>
              </button>
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
      <Modal
        title="Đăng xuất"
        onCancel={() => setIsModalOpen(false)}
        open={isModalOpen}
        onOk={handleLogout}
        okText="Đăng xuất"
        cancelText="Trở lại"
        confirmLoading={loading}
      >
        <p>
          Sau khi đăng xuất, bạn sẽ bị giới hạn nội dung. Bạn có muốn tiếp tục
          chứ?
        </p>
      </Modal>
    </div>
  );
};

export default ProfileAvatar;
