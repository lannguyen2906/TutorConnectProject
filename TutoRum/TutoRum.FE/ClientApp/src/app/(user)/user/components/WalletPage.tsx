import { Tabs } from "antd";
import { FileText } from "lucide-react";
import React from "react";
import { FaMoneyBill } from "react-icons/fa";
import BillsTable from "./BillsTable";
import PaymentRequestsTable from "./PaymentRequestsTable";
import PaymentRequestButton from "./PaymentRequestButton";
import WalletOverview from "./WalletOverview";

const WalletPage = () => {
  return (
    <div>
      <WalletOverview />
      <div className="mt-5">
        <Tabs
          type="card"
          items={[
            {
              key: "1",
              label: `Hóa đơn`,
              icon: <FileText size={16} />,
              children: <BillsTable type="teaching" />,
            },
            {
              key: "2",
              label: `Yêu cầu rút tiền`,
              icon: <FaMoneyBill size={16} />,
              children: <PaymentRequestsTable />,
            },
          ]}
          tabBarExtraContent={{
            right: <PaymentRequestButton />,
          }}
        />
      </div>
    </div>
  );
};

export default WalletPage;
