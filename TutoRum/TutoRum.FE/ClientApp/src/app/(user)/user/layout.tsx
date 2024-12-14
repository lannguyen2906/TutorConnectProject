import LinkAnchorHorizontal from "./components/LinkAnchorHorizontal";
import { userLinkList } from "./settings/settingLinkList";

const layout = ({ children }: { children: React.ReactNode }) => {
  return (
    <div className="w-full mt-5">
      <LinkAnchorHorizontal
        linkList={userLinkList(20, ["user-profile", "wallet"])}
      />
      <div>{children}</div>
    </div>
  );
};

export default layout;
