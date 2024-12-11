import "./globals.css";
import { Roboto } from "next/font/google";
import { ReactQueryClientProvider } from "@/components/provider/ReactQueryClientProvider";
import AppProvider from "@/components/provider/app-provider";
import { AntdRegistry } from "@ant-design/nextjs-registry";
import { ConfigProvider } from "antd";
import { Bounce, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

const roboto = Roboto({
  subsets: ["vietnamese"],
  weight: ["400", "500", "700"],
});

export const metadata = {
  title: "TutorConnect",
  description: "Cầu nối gia sư",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <ReactQueryClientProvider>
      <html lang="en" suppressHydrationWarning={true}>
        <body className={roboto.className}>
          <AntdRegistry>
            <ConfigProvider
              theme={{
                token: {
                  colorPrimary: "#6556FF",
                  colorPrimaryText: "white",
                  colorLink: "#6556FF",
                  borderRadius: 8,
                },
                components: {
                  Menu: {
                    itemSelectedBg: "#222C44",
                    itemSelectedColor: "white",
                  },
                },
              }}
            >
              <AppProvider>{children}</AppProvider>
            </ConfigProvider>
          </AntdRegistry>
          <ToastContainer
            stacked
            position="bottom-right"
            autoClose={5000}
            hideProgressBar={false}
            newestOnTop={false}
            closeOnClick
            rtl={false}
            pauseOnFocusLoss
            draggable
            pauseOnHover
            theme="light"
            transition={Bounce}
          />
        </body>
      </html>
    </ReactQueryClientProvider>
  );
}
