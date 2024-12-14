"use client";
// components/MarkdownRenderer.tsx
import React from "react";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import remarkBreaks from "remark-breaks";
import Image from "next/image";
import { cn } from "@/lib/utils";

interface MarkdownRendererProps {
  content: string;
  maxHeight?: number;
}

const MarkdownRenderer: React.FC<MarkdownRendererProps> = ({
  content,
  maxHeight,
}) => {
  const [full, setFull] = React.useState(maxHeight == null);

  return (
    <>
      <div
        className={cn(
          "markdown-body overflow-hidden transition-all ease-in-out duration-1000"
        )}
        style={
          !!maxHeight ? { maxHeight: full ? "100vh" : `${maxHeight}px` } : {}
        }
      >
        <ReactMarkdown
          // Sử dụng các plugin để hỗ trợ Markdown mở rộng
          remarkPlugins={[remarkGfm, remarkBreaks]}
          components={{
            img: ({ node, ...props }) => (
              // Thay đổi component img mặc định bằng component Image của Next.js để tối ưu hóa
              <Image
                src={props.src || ""}
                alt={props.alt || ""}
                layout="responsive"
                width={700}
                height={475}
              />
            ),
            a: ({ node, ...props }) => (
              <a
                href={props.href || ""}
                target="_blank"
                rel="noopener noreferrer"
              >
                {props.children}
              </a>
            ),
          }}
        >
          {content}
        </ReactMarkdown>
      </div>
      {maxHeight && (
        <button
          type="button"
          className="underline mt-2 hover:text-Blueviolet transition-all ease-in-out duration-300"
          onClick={() => setFull(!full)}
        >
          {full ? "Ẩn" : "Hiện thêm"}
        </button>
      )}
    </>
  );
};

export default MarkdownRenderer;
