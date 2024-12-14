import LinksAnchor from "@/app/(user)/user/settings/user-profile/components/LinksAnchor";
import { ScrollArea } from "@/components/ui/scroll-area";
import { settingList } from "./components/SettingList";
import { ContentLayout } from "../../components/content-layout";

const layout = ({ children }: { children: React.ReactNode }) => {
  return (
    <ContentLayout title="Cài đặt hệ thống">
      <div className="container lg:flex lg:gap-10 mt-10">
        <div className="lg:w-1/6">
          <LinksAnchor linkList={settingList} />
        </div>
        <div className="w-full lg:w-5/6">
          <ScrollArea>{children}</ScrollArea>
        </div>
      </div>
    </ContentLayout>
  );
};

export default layout;
