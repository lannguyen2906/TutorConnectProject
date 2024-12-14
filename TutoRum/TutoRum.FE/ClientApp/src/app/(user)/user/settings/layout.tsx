import { ScrollArea } from "@/components/ui/scroll-area";
import LinksAnchor from "./user-profile/components/LinksAnchor";
import { settingList } from "./settingLinkList";

const layout = ({ children }: { children: React.ReactNode }) => {
  return (
    <div className="container lg:flex lg:gap-10 mt-10">
      <div className="lg:w-1/6">
        <LinksAnchor linkList={settingList} />
      </div>
      <div className="w-full lg:w-5/6">
        <ScrollArea>{children}</ScrollArea>
      </div>
    </div>
  );
};

export default layout;
