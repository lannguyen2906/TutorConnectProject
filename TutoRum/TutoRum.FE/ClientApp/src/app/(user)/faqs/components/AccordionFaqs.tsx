"use client";
import MarkdownRenderer from "@/components/markdown/MarkdownRenderer";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { Input } from "antd";
import { Search } from "lucide-react";
import React, { useState } from "react";

interface FaqsI {
  id: number;
  question: string;
  answer: string;
}

const faqsMock: FaqsI[] = [
  {
    id: 1,
    question: "Làm thế nào để tìm gia sư phù hợp?",
    answer: `
**Để tìm gia sư phù hợp, bạn có thể thực hiện các bước sau:**

1. Truy cập vào trang web **_TutorConnect_**.
2. Sử dụng chức năng **tìm kiếm** để nhập môn học hoặc kỹ năng mà bạn cần.
3. Duyệt qua danh sách gia sư và xem hồ sơ của họ.
4. Liên hệ với gia sư mà bạn cảm thấy phù hợp để **thảo luận chi tiết**.`,
  },
  {
    id: 2,
    question: "Các môn học nào có sẵn trên TutorConnect?",
    answer: `
**TutorConnect** cung cấp nhiều môn học khác nhau, bao gồm nhưng không giới hạn ở:

- _Toán_
- _Lý_
- _Hóa_
- _Văn_
- _Tiếng Anh_
- _Lập trình_
- _Nghệ thuật_
- _Khoa học xã hội_

Bạn có thể tìm kiếm theo **tên môn học** trên trang chủ để xem danh sách gia sư có sẵn.`,
  },
  {
    id: 3,
    question: "Tôi có thể đánh giá gia sư sau khi hoàn thành khóa học không?",
    answer: `
**Có**, bạn có thể đánh giá gia sư sau khi hoàn thành khóa học. Điều này giúp các học viên khác có thêm thông tin để chọn gia sư phù hợp.

Để đánh giá, bạn chỉ cần truy cập vào **hồ sơ của gia sư** và chọn **đánh giá** mà bạn muốn.`,
  },
  {
    id: 4,
    question: "Làm thế nào để thanh toán cho khóa học?",
    answer: `
Bạn có thể thanh toán cho khóa học thông qua các phương thức sau:

- **Thẻ tín dụng**
- **Ví điện tử**
- **Chuyển khoản ngân hàng**

Sau khi chọn gia sư và xác nhận lịch học, bạn sẽ được hướng dẫn chi tiết về **quy trình thanh toán**.`,
  },
  {
    id: 5,
    question: "Tôi có thể hủy hoặc thay đổi lịch học không?",
    answer: `
**Có**, bạn có thể hủy hoặc thay đổi lịch học với gia sư của mình. Tuy nhiên, bạn cần thông báo cho gia sư **ít nhất 24 giờ trước giờ học** để tránh bị tính phí.

Bạn có thể thực hiện việc này thông qua **trang cá nhân** của mình trên **TutorConnect**.`,
  },
  {
    id: 6,
    question: "Tôi có thể liên hệ với gia sư trước khi đặt lịch học không?",
    answer: `
**Có**, bạn có thể liên hệ với gia sư qua chức năng **nhắn tin** trên trang web trước khi đặt lịch học.

Điều này giúp bạn có thể thảo luận về **yêu cầu học tập** và **phương pháp dạy** của gia sư trước khi quyết định.`,
  },
];

const AccordionFaqs = () => {
  const [searchQuery, setSearchQuery] = useState("");
  const [openItems, setOpenItems] = useState<string[]>([]);

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value.toLowerCase();
    setSearchQuery(value);

    // Tìm kiếm các câu hỏi hoặc câu trả lời có chứa từ khóa
    const matchingItems = faqsMock
      .filter(
        (faq) =>
          faq.question.toLowerCase().includes(value) ||
          faq.answer.toLowerCase().includes(value)
      )
      .map((faq) => faq.id.toString());

    if (value === "") {
      setOpenItems([]);
    } else {
      setOpenItems(matchingItems); // Mở accordion cho các mục có chứa từ khóa
    }
  };

  const highlightText = (text: string, query: string) => {
    if (!query) return text;
    const regex = new RegExp(`(${query})`, "gi");
    return text.replace(regex, "<span class='bg-grey500'>$1</span>");
  };

  const highlightInMarkdown = (text: string, query: string) => {
    if (!query) return text;
    const regex = new RegExp(`(${query})`, "gi");
    return text.replace(regex, "`$1`");
  };

  return (
    <div className="w-full">
      <div className="flex justify-center my-5">
        <Input
          prefix={<Search />}
          allowClear
          style={{ width: "50%" }}
          value={searchQuery}
          onChange={handleSearchChange}
          placeholder="Nhập từ khóa bạn muốn tìm kiếm"
        />
      </div>
      <Accordion className="w-full" type="multiple" value={openItems}>
        {faqsMock.map((faq) => (
          <AccordionItem key={faq.id} value={faq.id.toString()}>
            <AccordionTrigger
              className="text-xl"
              onClick={() => {
                setOpenItems((prv) =>
                  prv.includes(faq.id.toString())
                    ? prv.filter((item) => item !== faq.id.toString())
                    : [...prv, faq.id.toString()]
                );
              }}
            >
              <span
                dangerouslySetInnerHTML={{
                  __html: highlightText(faq.question, searchQuery),
                }}
              />
            </AccordionTrigger>
            <AccordionContent>
              <MarkdownRenderer
                content={highlightInMarkdown(faq.answer, searchQuery)}
              />
            </AccordionContent>
          </AccordionItem>
        ))}
      </Accordion>
    </div>
  );
};

export default AccordionFaqs;
