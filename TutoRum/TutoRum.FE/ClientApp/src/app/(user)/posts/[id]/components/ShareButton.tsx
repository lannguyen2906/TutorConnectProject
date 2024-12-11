"use client";
import { useState } from "react";
import { Button } from "@/components/ui/button"; // Using shadcn UI Button
import { Copy, Share2, Check } from "lucide-react"; // Icon from Lucide React
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTrigger,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { toast } from "react-toastify";

const ShareButton = () => {
  const [isCopied, setIsCopied] = useState(false); // State to track if the URL is copied
  const currentUrl = typeof window !== "undefined" ? window.location.href : "";

  const copyToClipboard = () => {
    navigator.clipboard.writeText(currentUrl);
    setIsCopied(true); // Set to true after copying
    toast.info("Copied to clipboard!");

    // Optionally reset the icon after 2 seconds
    setTimeout(() => {
      setIsCopied(false);
    }, 2000);
  };

  return (
    <Dialog>
      <DialogTrigger>
        <Button variant={"outline"} className="flex items-center">
          <Share2 />
        </Button>
      </DialogTrigger>
      <DialogContent>
        <DialogHeader className="space-y-5">
          <DialogTitle>Sao chép đường dẫn tại đây</DialogTitle>
          <DialogDescription className="flex gap-5">
            <Input value={currentUrl} readOnly />
            <Button variant={"outline"} onClick={copyToClipboard}>
              {isCopied ? <Check /> : <Copy />}{" "}
              {/* Toggle between Copy and Check icon */}
            </Button>
          </DialogDescription>
        </DialogHeader>
      </DialogContent>
    </Dialog>
  );
};

export default ShareButton;
