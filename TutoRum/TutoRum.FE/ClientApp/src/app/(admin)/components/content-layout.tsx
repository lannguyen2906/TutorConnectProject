import { Navbar } from "@/app/(admin)/components/navbar";
import CustomizedBreadcrumb from "@/app/(user)/components/Breadcrumb/CustomizedBreadcrumb";

interface ContentLayoutProps {
  title: string;
  children: React.ReactNode;
}

export function ContentLayout({ title, children }: ContentLayoutProps) {
  return (
    <div className="h-screen">
      <Navbar title={title} />
      <div className="mx-32 pt-8 pb-8">
        <div>
          <CustomizedBreadcrumb currentpage={title} />
          <div className="w-full">{children}</div>
        </div>
      </div>
    </div>
  );
}
