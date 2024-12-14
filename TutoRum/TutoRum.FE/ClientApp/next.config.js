/** @type {import('next').NextConfig} */
const nextConfig = {
  async redirects() {
    return [
      {
        source: "/",
        destination: "/home",
        permanent: true,
      },
      {
        source: "/settings",
        destination: "/user/settings/user-profile",
        permanent: true,
      },
      {
        source: "/admin/settings",
        destination: "/admin/settings/rate-ranges",
        permanent: true,
      },
      {
        source: "/user",
        destination: "/user/settings/user-profile",
        permanent: true,
      },
    ];
  },
  reactStrictMode: false,
  trailingSlash: true, // thêm dấu `/` vào cuối URL để đảm bảo đồng nhất khi truy cập vào các đường dẫn

  images: {
    remotePatterns: [
      {
        protocol: "https",
        hostname: "octodex.github.com",
      },
      {
        protocol: "https",
        hostname: "github.com",
      },
      {
        protocol: "https",
        hostname: "firebasestorage.googleapis.com",
      },
      {
        protocol: "https",
        hostname: "www.readingrockets.org",
      },
      {
        protocol: "https",
        hostname: "img.freepik.com",
      },
      {
        protocol: "https",
        hostname: "api.vietqr.io",
      },
      {
        protocol: "https",
        hostname: "vietqr.net",
      },
    ],
  },
};

module.exports = nextConfig;
