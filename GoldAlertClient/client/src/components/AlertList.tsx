import { Trash2, ArrowUpCircle, ArrowDownCircle, AlertCircle } from "lucide-react";
import { format } from "date-fns";
import { motion, AnimatePresence } from "framer-motion";
import { useAlerts, useDeleteAlert } from "@/hooks/use-alerts";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";

const formatCurrency = (value: number) =>
  new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(value);

export function AlertList() {
  const { data: alerts, isLoading } = useAlerts();
  const { mutate: deleteAlert, isPending: isDeleting } = useDeleteAlert();

  if (isLoading) {
    return (
      <Card className="border-none shadow-none bg-transparent">
        <CardHeader className="px-0">
          <CardTitle className="text-lg">Danh sách cảnh báo</CardTitle>
        </CardHeader>
        <CardContent className="px-0 space-y-3">
          <Skeleton className="h-16 w-full rounded-xl" />
          <Skeleton className="h-16 w-full rounded-xl" />
          <Skeleton className="h-16 w-full rounded-xl" />
        </CardContent>
      </Card>
    );
  }

  if (!alerts || alerts.length === 0) {
    return (
      <Card className="border-dashed border-2 border-green-200 bg-green-50/50 shadow-none">
        <CardContent className="flex flex-col items-center justify-center py-12 text-center text-green-700">
          <AlertCircle className="w-12 h-12 mb-4 opacity-20" />
          <h3 className="text-lg font-semibold mb-1">Chưa có cảnh báo nào</h3>
          <p className="text-sm opacity-60 max-w-[250px]">
            Hãy tạo cảnh báo giá đầu tiên để không bỏ lỡ cơ hội đầu tư.
          </p>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="border-none shadow-none bg-transparent">
      <CardHeader className="px-0 pt-0">
        <div className="flex items-center justify-between">
          <CardTitle className="text-xl text-green-900">Danh sách cảnh báo ({alerts.length})</CardTitle>
        </div>
      </CardHeader>
      <CardContent className="px-0">
        <div className="space-y-3">
          <AnimatePresence>
            {alerts.map((alert) => (
              <motion.div
                key={alert.id}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, height: 0, marginTop: 0 }}
                transition={{ duration: 0.2 }}
                className="group relative overflow-hidden rounded-xl border border-green-100 bg-white shadow-sm hover:shadow-md hover:border-yellow-200 transition-all duration-200"
              >
                <div className="absolute left-0 top-0 bottom-0 w-1.5 bg-gradient-to-b from-yellow-400 to-yellow-600" />
                
                <div className="flex items-center justify-between p-4 pl-6">
                  <div className="flex items-center gap-4">
                    <div className={`p-2 rounded-full ${alert.condition === 'above' ? 'bg-green-100 text-green-600' : 'bg-red-100 text-red-600'}`}>
                      {alert.condition === 'above' ? (
                        <ArrowUpCircle className="w-6 h-6" />
                      ) : (
                        <ArrowDownCircle className="w-6 h-6" />
                      )}
                    </div>
                    
                    <div>
                      <div className="flex items-baseline gap-2">
                        <span className="text-xs font-semibold text-gray-500 uppercase tracking-wide">
                          {alert.condition === 'above' ? 'Khi giá ≥' : 'Khi giá ≤'}
                        </span>
                        <span className="text-lg font-bold font-mono text-green-950">
                          {formatCurrency(alert.targetPrice)}
                        </span>
                      </div>
                      <div className="text-xs text-gray-400 mt-0.5">
                        Tạo lúc: {alert.createdAt ? format(new Date(alert.createdAt), "HH:mm dd/MM/yyyy") : 'N/A'}
                      </div>
                    </div>
                  </div>

                  <AlertDialog>
                    <AlertDialogTrigger asChild>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="text-gray-400 hover:text-red-500 hover:bg-red-50 transition-colors"
                        disabled={isDeleting}
                      >
                        <Trash2 className="w-5 h-5" />
                      </Button>
                    </AlertDialogTrigger>
                    <AlertDialogContent>
                      <AlertDialogHeader>
                        <AlertDialogTitle>Xác nhận xóa cảnh báo</AlertDialogTitle>
                        <AlertDialogDescription>
                          Bạn có chắc chắn muốn xóa cảnh báo giá {formatCurrency(alert.targetPrice)} không?
                        </AlertDialogDescription>
                      </AlertDialogHeader>
                      <AlertDialogFooter>
                        <AlertDialogCancel>Hủy</AlertDialogCancel>
                        <AlertDialogAction
                          className="bg-red-500 hover:bg-red-600 text-white"
                          onClick={() => deleteAlert(alert.id)}
                        >
                          Xóa cảnh báo
                        </AlertDialogAction>
                      </AlertDialogFooter>
                    </AlertDialogContent>
                  </AlertDialog>
                </div>
              </motion.div>
            ))}
          </AnimatePresence>
        </div>
      </CardContent>
    </Card>
  );
}
