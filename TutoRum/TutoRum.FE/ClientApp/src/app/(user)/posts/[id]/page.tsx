"use client";
import PostDetail from "./components/PostDetail";
import { usePostDetail } from "@/hooks/use-posts";
import CustomizedBreadcrumb from "../../components/Breadcrumb/CustomizedBreadcrumb";

type Props = {
  params: { id: string };
};

// export async function generateMetadata(
//   { params, searchParams }: Props,
//   parent: ResolvingMetadata
// ): Promise<Metadata> {
//   const { payload } = await getDetail(Number(params.id));
//   const product = payload.data;
//   const url = envConfig.NEXT_PUBLIC_URL + "/products/" + product.id;
//   return {
//     title: product.name,
//     description: product.description,
//     openGraph: {
//       ...baseOpenGraph,
//       title: product.name,
//       description: product.description,
//       url,
//       images: [
//         {
//           url: product.image,
//         },
//       ],
//     },
//     alternates: {
//       canonical: url,
//     },
//   };
// }

export default function PostDetailPage({ params }: Props) {
  const { data, isLoading } = usePostDetail(Number(params.id));

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="container mt-5">
      <CustomizedBreadcrumb currentpage={data?.title || "Bài viết"} />
      <PostDetail {...data} />
    </div>
  );
}
