import { Disclosure } from "@headlessui/react";
import Link from "next/link";
import React, { useEffect, useState } from "react";
import { Bars3Icon } from "@heroicons/react/24/outline";
import Image from "next/image";
import { useAppContext } from "@/components/provider/app-provider";
import ProfileAvatar from "./ProfileAvatar";
import Drawerdata from "./Drawerdata";
import { usePathname, useRouter } from "next/navigation";
import Logo from "@/components/ui/logo";
import { Drawer } from "antd";
import { BookOpen, GraduationCap, Home, Newspaper, Search } from "lucide-react";
interface NavigationItem {
  name: string;
  href: string;
  icon: JSX.Element;
}

export const navigation: NavigationItem[] = [
  { name: "Trang chủ", href: "/home", icon: <Home /> },
  { name: "Yêu cầu gia sư", href: "/tutor-requests", icon: <BookOpen /> },
  { name: "Tìm gia sư", href: "/tutors", icon: <Search /> },
  {
    name: "Đăng ký làm gia sư",
    href: "/tutor-register",
    icon: <GraduationCap />,
  },
  { name: "Bài viết", href: "/posts", icon: <Newspaper /> },
];

function classNames(...classes: string[]) {
  return classes.filter(Boolean).join(" ");
}

const Navbar = () => {
  const router = useRouter();
  const pathName = usePathname();

  const { user } = useAppContext();

  const [isOpen, setIsOpen] = React.useState(false);

  return (
    <Disclosure as="nav" className="navbar" style={{ zIndex: 50 }}>
      <>
        <div className="mx-auto max-w-full px-4 py-6 lg:px-8">
          <div className="relative flex h-12 md:h-14 items-center justify-between">
            <div className="flex flex-1 items-center sm:items-stretch sm:justify-start">
              {/* LOGO */}
              <button onClick={() => router.push("/")}>
                <Logo hasText={true} size={250} />
              </button>

              {/* LINKS */}

              <div className="hidden xl:block m-auto">
                <div className="flex space-x-4">
                  {navigation.map((item) => (
                    <Link key={item.name} href={item.href}>
                      <span
                        className={classNames(
                          pathName.startsWith(item.href)
                            ? "underline-links"
                            : "text-slategray",
                          "px-3 py-4 text-sm opacity-75 hover:opacity-100"
                        )}
                        aria-current={item.href ? "page" : undefined}
                      >
                        {item.name}
                      </span>
                    </Link>
                  ))}
                </div>
              </div>
            </div>

            <div className="hidden xl:flex">
              {user ? (
                <div className="flex gap-4 items-center">
                  <ProfileAvatar />
                </div>
              ) : (
                <>
                  <div className="absolute inset-y-0 right-0 flex items-center sm:static sm:inset-auto sm:pr-0">
                    <div className="hidden xl:block">
                      <button
                        type="button"
                        className="text-base text-Blueviolet font-medium"
                        onClick={() => router.push("/signup")}
                      >
                        Đăng ký
                      </button>
                    </div>
                  </div>

                  <div className="absolute inset-y-0 right-0 flex items-center pr-2 sm:static sm:inset-auto sm:pr-0">
                    <div className="hidden xl:block">
                      <button
                        className="text-Blueviolet text-base font-medium ml-9 py-5 px-8 transition duration-150 ease-in-out rounded-full bg-semiblueviolet hover:text-white hover:bg-Blueviolet"
                        onClick={() => router.push("/login")}
                      >
                        Đăng nhập
                      </button>
                    </div>
                  </div>
                </>
              )}
            </div>

            {/* DRAWER FOR MOBILE VIEW */}

            {/* DRAWER ICON */}

            <div className="block xl:hidden">
              <Bars3Icon
                className="block h-6 w-6"
                aria-hidden="true"
                onClick={() => setIsOpen(true)}
              />
            </div>

            {/* DRAWER LINKS DATA */}

            <Drawer
              open={isOpen}
              onClose={() => setIsOpen(false)}
              placement="left"
              title="TutorConnect"
              width={300}
              className="p-0"
            >
              <Drawerdata setIsOpen={setIsOpen} />
            </Drawer>
          </div>
        </div>
      </>
    </Disclosure>
  );
};

export default Navbar;
