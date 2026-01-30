import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { BellPlus, Loader2 } from "lucide-react";
import { useCreateAlert } from "@/hooks/use-alerts";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { insertAlertSchema } from "@shared/schema";

// Extend schema for form usage if needed, mostly for coerce
const formSchema = insertAlertSchema.extend({
  targetPrice: z.coerce.number().min(1000, "Giá phải lớn hơn 1000"),
});

export function AlertForm() {
  const { mutate: createAlert, isPending } = useCreateAlert();

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      targetPrice: 83000000,
      condition: "above",
    },
  });

  function onSubmit(values: z.infer<typeof formSchema>) {
    createAlert(values, {
      onSuccess: () => {
        // Reset form but keep condition
        form.reset({
          targetPrice: values.targetPrice,
          condition: values.condition,
        });
      }
    });
  }

  return (
    <Card className="shadow-lg border-green-100/50">
      <CardHeader className="bg-gradient-to-r from-green-50 to-white border-b border-green-50">
        <CardTitle className="flex items-center gap-2 text-green-900">
          <BellPlus className="w-5 h-5 text-yellow-600" />
          Tạo Cảnh Báo Mới
        </CardTitle>
        <CardDescription>
          Nhận thông báo ngay khi giá vàng đạt mức mong muốn.
        </CardDescription>
      </CardHeader>
      <CardContent className="pt-6">
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="targetPrice"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-green-800 font-semibold">Giá mục tiêu (VND)</FormLabel>
                    <FormControl>
                      <div className="relative">
                        <Input
                          type="number"
                          placeholder="Nhập giá vàng..."
                          className="pl-4 pr-24 h-11 text-lg font-semibold bg-white border-green-200 focus:border-yellow-500 focus:ring-yellow-500/20"
                          {...field}
                        />
                        <div className="absolute right-3 top-2.5 text-xs font-medium text-gray-400 pointer-events-none">
                          VND/lượng
                        </div>
                      </div>
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="condition"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-green-800 font-semibold">Điều kiện kích hoạt</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger className="h-11 bg-white border-green-200 focus:border-yellow-500 focus:ring-yellow-500/20">
                          <SelectValue placeholder="Chọn điều kiện" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="above">Khi giá tăng CAO hơn (≥)</SelectItem>
                        <SelectItem value="below">Khi giá giảm THẤP hơn (≤)</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <Button
              type="submit"
              className="w-full h-12 text-base font-bold bg-yellow-500 hover:bg-yellow-600 text-green-950 shadow-lg shadow-yellow-500/20 transition-all duration-300 hover:-translate-y-0.5"
              disabled={isPending}
            >
              {isPending ? (
                <><Loader2 className="mr-2 h-5 w-5 animate-spin" /> Đang thiết lập...</>
              ) : (
                "Thiết lập Cảnh báo"
              )}
            </Button>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
