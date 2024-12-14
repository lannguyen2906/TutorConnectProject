import React, { Dispatch, SetStateAction } from "react";
import Link from "next/link";
import { useAppContext } from "@/components/provider/app-provider";
import { usePathname, useRouter } from "next/navigation";
import { tConnectService } from "@/utils/services/tConnectService";
import { navigation } from "./Navbar";
import ProfileAvatar from "./ProfileAvatar";
import { Separator } from "@/components/ui/separator";
import { Button } from "antd";
import { LogOut } from "lucide-react";
import { userLinkList } from "../../user/settings/settingLinkList";

interface NavigationItem {
  name: string;
  href: string;
  current: boolean;
}

function classNames(...classes: string[]) {
  return classes.filter(Boolean).join(" ");
}

const Data = ({
  setIsOpen,
}: {
  setIsOpen: Dispatch<SetStateAction<boolean>>;
}) => {
  const router = useRouter();
  const pathname = usePathname();
  const { user, refetch, remove } = useAppContext();
  const handleLogout = async () => {
    try {
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
      router.push("/login");
      router.refresh();
      localStorage.removeItem("sessionToken");
      localStorage.removeItem("sessionTokenExpiresAt");
    }
  };

  return (
    <div className="flex flex-col justify-between h-full">
      <div className="space-y-2">
        {user && (
          <>
            <ProfileAvatar />
            <Separator />
          </>
        )}

        {navigation.map((item) => (
          <Link
            key={item.name}
            href={item.href}
            className={classNames(
              pathname.startsWith(item.href)
                ? "text-Blueviolet border-b-2 border-Blueviolet"
                : "text-black",
              "py-3 flex items-center gap-4"
            )}
            aria-current={item.href === pathname ? "page" : undefined}
            onClick={() => setIsOpen(false)}
          >
            {item.icon}
            {item.name}
          </Link>
        ))}
      </div>
      <Separator />
      <div className="mb-4">
        {userLinkList(24, ["user-profile"]).map(
          ({ key, label, icon, href, roles }) => {
            const matchedRoles = user?.roles?.filter((role) =>
              roles.includes(role)
            );
            if (matchedRoles && matchedRoles.length > 0) {
              return (
                <Link
                  key={key}
                  href={href}
                  className={classNames(
                    pathname == `${href}/`
                      ? "text-Blueviolet border-b-2 border-Blueviolet"
                      : "text-black",
                    "py-3 flex items-center gap-4"
                  )}
                  aria-current={href === pathname ? "page" : undefined}
                  onClick={() => setIsOpen(false)}
                >
                  {icon}
                  {label}
                </Link>
              );
            }
          }
        )}
      </div>
      {user ? (
        <>
          <Button
            type="primary"
            icon={<LogOut size={16} />}
            className="bg-white w-full text-Blueviolet border border-semiblueviolet font-medium py-2 px-4 rounded"
            onClick={() => handleLogout()}
          >
            Đăng xuất
          </Button>
        </>
      ) : (
        <div>
          <button
            className="bg-white w-full text-Blueviolet border border-semiblueviolet font-medium py-2 px-4 rounded"
            onClick={() => router.push("/login")}
          >
            Đăng nhập
          </button>
          <button
            className="bg-semiblueviolet w-full hover:bg-Blueviolet hover:text-white text-Blueviolet font-medium my-2 py-2 px-4 rounded"
            onClick={() => router.push("/signup")}
          >
            Đăng ký
          </button>
        </div>
      )}
    </div>
  );
};

export default Data;
