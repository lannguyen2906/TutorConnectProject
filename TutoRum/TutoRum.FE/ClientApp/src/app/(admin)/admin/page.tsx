import { ContentLayout } from "@/app/(admin)/components/content-layout";
import DashboardPage from "./components/DashboardPage";

export default function Page() {
  return (
    <ContentLayout title="Dashboard">
      <DashboardPage />
    </ContentLayout>
  );
}
