"use client";
import { useSubjectList } from "@/hooks/use-subject";
import { Carousel } from "antd";

const Companies = () => {
  const { data } = useSubjectList();

  return (
    <div className="text-center mt-16 bg-lightblue py-12 container">
      <div className="w-full">
        {/* Tiêu đề */}
        <h2 className="text-midnightblue text-3xl font-bold mb-8">
          Các môn học phổ biến của chúng tôi
        </h2>

        {/* Carousel */}
        <div className="border-b-2 border-gray-200 py-8">
          <Carousel
            autoplay
            dots={false}
            slidesToShow={4} // Hiển thị 4 mục cùng lúc
            slidesToScroll={1}
            autoplaySpeed={2500}
            speed={500}
            responsive={[
              {
                breakpoint: 768, // Kích thước màn hình nhỏ hơn 768px
                settings: {
                  slidesToShow: 2, // Hiển thị 2 slides trên màn hình nhỏ
                  slidesToScroll: 1,
                },
              },
              {
                breakpoint: 480, // Kích thước màn hình nhỏ hơn 480px
                settings: {
                  slidesToShow: 1, // Hiển thị 1 slide trên màn hình rất nhỏ
                  slidesToScroll: 1,
                },
              },
            ]}
          >
            {data?.map((subject) => (
              <div
                key={subject.subjectId}
                className="flex justify-center items-center"
              >
                <h3 className="text-2xl font-semibold text-muted-foreground mb-2">
                  {subject.subjectName}
                </h3>
              </div>
            ))}
          </Carousel>
        </div>
      </div>
    </div>
  );
};

export default Companies;
