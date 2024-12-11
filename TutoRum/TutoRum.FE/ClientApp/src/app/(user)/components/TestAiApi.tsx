"use client";
import React, { useState } from "react";
import OpenAI from "openai";
import { Avatar, Button, Input, Popover, Spin, Tooltip } from "antd";
import TextArea from "antd/es/input/TextArea";
import MarkdownRenderer from "@/components/markdown/MarkdownRenderer";
import StaggeredFadeLoader from "@/components/ui/staggered-fade-loader";
import { Send, SendHorizonal } from "lucide-react";
import { ResizableBox } from "react-resizable";
import "react-resizable/css/styles.css";
import { Form } from "@/components/ui/form";
import { useForm } from "react-hook-form";
import { cn } from "@/lib/utils";
import { messages } from "./AiMessage";

const TestAiApi = () => {
  const [request, setRequest] = useState("");
  const [aiResponse, setAiResponse] = useState("Xin chào TutoUser!");
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);

  const client = new OpenAI({
    baseURL: "https://models.inference.ai.azure.com",
    apiKey: "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",
    dangerouslyAllowBrowser: true,
  });

  const form = useForm();

  const handleClick = async () => {
    if (!request.trim()) return; // Bỏ qua nếu không có nội dung nhập

    const findRequestIndex = messages.findIndex(
      (message) => message.role === "user" && message.content === request
    );

    setLoading(true);
    try {
      if (findRequestIndex !== -1) {
        const response = messages[findRequestIndex + 1];
        setAiResponse(response?.content as string);
        return;
      } else {
        const response = await client.chat.completions.create({
          messages: [
            ...messages,
            {
              role: "user",
              content: request,
            },
          ],
          model: "gpt-4o",
          temperature: 0.7,
          max_tokens: 1000,
          top_p: 1,
        });

        if (response.choices.length > 0) {
          setAiResponse(
            response.choices?.at(0)?.message.content ?? "Không có phản hồi."
          );
        }
      }
    } catch (error) {
      setAiResponse("Có lỗi xảy ra! Vui lòng thử lại sau.");
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleClick();
    }
  };
  return (
    <div className="hidden lg:block">
      <Popover
        content={
          <ResizableBox
            width={400}
            height={400}
            minConstraints={[400, 400]}
            axis="x"
          >
            <div className="relative flex flex-col gap-4 w-full h-full overflow-hidden items-end">
              <div className="flex gap-4 h-3/4 w-fit max-w-full overflow-auto">
                <div
                  className={cn(
                    "flex-1 bg-semiblueviolet text-midnightblue rounded-lg px-3 h-fit py-1",
                    loading && "py-3"
                  )}
                >
                  {loading ? (
                    <StaggeredFadeLoader />
                  ) : (
                    <MarkdownRenderer content={aiResponse} />
                  )}
                </div>
                <Avatar src="/tutoBot.png" size="large" />
              </div>
              <Form {...form}>
                <form
                  onSubmit={form.handleSubmit(handleClick)}
                  className="h-1/4 w-full flex items-center gap-4"
                >
                  <TextArea
                    className="flex-1"
                    onChange={(e) => setRequest(e.target.value)}
                    value={request}
                    placeholder="Nhập câu hỏi của bạn"
                    rows={4}
                    style={{ resize: "none", paddingInline: "1px" }}
                    allowClear
                    onKeyDown={handleKeyDown}
                  />
                  <Button
                    className="me-1"
                    type="primary"
                    htmlType="submit"
                    disabled={loading}
                    loading={loading}
                    icon={<SendHorizonal size={16} />}
                  />
                </form>
              </Form>
            </div>
          </ResizableBox>
        }
        trigger="click"
        placement="right"
        title="TutoBot có thể giúp gì cho bạn?"
        open={open}
        onOpenChange={setOpen}
      >
        <Button
          type="primary"
          shape="circle"
          icon={<Avatar src="/tutoBot.png" size="large" />}
          style={{
            position: "fixed",
            top: "50%",
            left: 20,
            width: 45,
            height: 45,
          }}
        />
      </Popover>
    </div>
  );
};

export default TestAiApi;
