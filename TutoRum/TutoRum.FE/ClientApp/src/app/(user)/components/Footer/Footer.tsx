import Link from "next/link";
import Image from "next/image";
import Logo from "@/components/ui/logo";

interface ProductType {
  id: number;
  section: string;
  link: NavigationItem[];
}

interface SocialLink {
  imgSrc: string;
  link: string;
  width: number;
}

interface NavigationItem {
  name: string;
  href: string;
}

const socialLinksData: SocialLink[] = [
  {
    imgSrc: "/assets/footer/facebook.svg",
    link: "https://www.facebook.com",
    width: 10,
  },
  {
    imgSrc: "/assets/footer/insta.svg",
    link: "https://www.instagram.com",
    width: 14,
  },
  {
    imgSrc: "/assets/footer/twitter.svg",
    link: "https://www.twitter.com",
    width: 14,
  },
];

const products: ProductType[] = [
  {
    id: 1,
    section: "Trung tâm",
    link: [
      { name: "Trang chủ", href: "/home" },
      { name: "Yêu cầu gia sư", href: "/tutor-requests" },
      { name: "Tìm gia sư", href: "/tutors" },
      { name: "Đăng ký làm gia sư", href: "/tutor-register" },
      { name: "Bài viết", href: "/posts" },
    ],
  },
  {
    id: 2,
    section: "Liên hệ",
    link: [
      { name: "Trợ giúp/FAQ", href: "/faqs" },
      { name: "Đối tác", href: "/partners" },
    ],
  },
];

const Footer = () => {
  return (
    <div className="mx-auto max-w-2xl sm:pt-24 px-4 sm:px-6 lg:max-w-7xl lg:px-8">
      <div className="my-12 grid grid-cols-1 gap-y-10 sm:grid-cols-6 lg:grid-cols-12">
        {/* COLUMN-1 */}
        <div className="sm:col-span-6 lg:col-span-5">
          <Logo hasText={true} />
          <h3 className="text-xs font-medium text-gunmetalgray lh-160 mt-5 mb-4 lg:mb-16">
            Tìm người bạn đồng hành cùng mình trên con đường <br /> khám phá bản
            thân.
          </h3>
          <div className="flex gap-4">
            {socialLinksData.map((item, i) => (
              <Link href={item.link} key={i}>
                <div className="bg-white h-12 w-12 shadow-xl text-base rounded-full flex items-center justify-center footer-icons hover:bg-ultramarine">
                  <Image
                    src={item.imgSrc}
                    alt="Social icon"
                    width={item.width}
                    height={item.width}
                    className="sepiaa"
                  />
                </div>
              </Link>
            ))}
          </div>
        </div>

        {/* CLOUMN-2/3/4 */}
        {products.map((product) => (
          <div key={product.id} className="sm:col-span-2">
            <p className="text-black text-lg font-medium mb-9">
              {product.section}
            </p>
            <ul>
              {product.link.map((link, index) => (
                <li key={index} className="mb-5">
                  <Link
                    href={link.href}
                    className="text-darkgray text-base font-normal mb-6 space-links"
                  >
                    {link.name}
                  </Link>
                </li>
              ))}
            </ul>
          </div>
        ))}
      </div>

      {/* All Rights Reserved */}
      <div className="py-10 md:flex items-center justify-between border-t border-t-gray-blue">
        <h4 className="text-dark-red opacity-75 text-sm text-center md:text-start font-normal">
          @2024.TutorConnect.All rights reserved
        </h4>
        <div className="flex gap-5 mt-5 md:mt-0 justify-center md:justify-start">
          <h4 className="text-dark-red opacity-75 text-sm font-normal">
            <Link href="/privacy-policy" target="_blank">
              Chính sách quyền riêng tư
            </Link>
          </h4>
          <div className="h-5 bg-dark-red opacity-25 w-0.5"></div>
          <h4 className="text-dark-red opacity-75 text-sm font-normal">
            <Link href="/terms-and-conditions" target="_blank">
              Điều khoản và điều kiện
            </Link>
          </h4>
        </div>
      </div>
    </div>
  );
};

export default Footer;
